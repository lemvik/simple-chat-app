import React, {Component} from 'react';
import {HubConnectionBuilder} from '@aspnet/signalr';

export class ChatRoom extends Component {
    constructor(props) {
        super(props);
        this.state = {members: [], messages: [], lastMessageId: 0, input: "", connected: false};
        this.alias = props.location.state.alias;

        let token = props.location.state.token;

        this.connection = new HubConnectionBuilder()
            .withUrl("/chat", {accessTokenFactory: () => token})
            .build();

        this.connection.start().then(function () {
            this.setState(() => {
                return {connected: true};
            })
        }.bind(this)).catch(function (error) {
            return console.error(error.toString());
        });

        this.connection.on("ReceiveMessage", this.addMessage.bind(this));
        this.connection.on("Presence", this.updatePresence.bind(this));
        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    addMessage(user, message) {
        this.setState((state) => {
            let newMessageId = state.lastMessageId + 1;
            return {
                messages: [...state.messages, {user: user, message: message, id: newMessageId}],
                lastMessageId: newMessageId
            };
        })
    }

    updatePresence(users) {
        this.setState({
            members: users.map(n => {
                return {user: n};
            })
        });
    }

    handleChange(event) {
        this.setState({input: event.target.value})
    }

    handleSubmit(event) {
        this.connection.invoke("SendMessage", this.alias, this.state.input).catch(err => console.error(err));
        this.setState({input: ""});
        event.preventDefault();
    }

    render() {
        if (!this.state.connected) {
            return (<p><em>Connecting...</em></p>);
        }

        let users = this.state.members.map(mem => <li key={mem.user}><span>{mem.user}</span></li>);

        let contents = this.state.messages.map(msg => <li key={msg.id}>
            <span>{msg.user}</span>: <span>{msg.message}</span></li>);

        return (
            <div className={'row fill'}>
                <div className={'col-sm-1'}>
                    <header>Users:</header>
                    <ul className={'list-unstyled'}>
                        {users}
                    </ul>
                </div>
                <div className={'col-sm-11'}>
                    <header>Chat:</header>
                    <ul className={'list-unstyled'}>
                        {contents}
                    </ul>
                    <div>
                        <form onSubmit={this.handleSubmit}>
                            <div className={'form-group'}>
                                <label>Enter message:</label>
                                <input type="text" value={this.state.input} className={'form-control'} id="chatMessage"
                                       onChange={this.handleChange}/>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        );
    }

    componentWillUnmount() {
        this.connection.stop().catch(err => console.error(err));
    }
}
