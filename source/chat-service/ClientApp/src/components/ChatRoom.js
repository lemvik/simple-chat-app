import React, {Component} from 'react';
import {HubConnectionBuilder} from '@aspnet/signalr';

export class ChatRoom extends Component {
    constructor(props) {
        super(props);
        this.state = {members: [], messages: [], input: "", connected: false};
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
        this.connection.on("UserConnected", this.addChatUser.bind(this));
        this.connection.on("UserDisconnected", this.removeChatUser.bind(this));
        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    addMessage(user, message) {
        this.setState((state) => {
            return {messages: [...state.messages, {user: user, message: message}]};
        })
    }

    addChatUser(user) {
        this.setState((state) => {
            return {members: [...state.members.filter(m => m.user !== user), {user: user}]};
        });
    }

    removeChatUser(user) {
        this.setState((state) => {
            return {members: state.members.filter(m => m.user !== user)};
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

        console.dir(this.state.messages);
        let contents = this.state.messages.map(msg => <li key={msg.message}>
            <span>{msg.user}</span>: <span>{msg.message}</span></li>);

        return (
            <div>
                <ul>
                    {users}
                </ul>
                <ul>
                    {contents}
                </ul>
                <div>
                    <form onSubmit={this.handleSubmit}>
                        <label>Enter message:</label>
                        <input type="text" value={this.state.input} id="chatMessage" onChange={this.handleChange}/>
                        <input type="submit" value="Send"/>
                    </form>
                </div>
            </div>
        );
    }

    componentWillUnmount() {
        this.connection.stop().catch(err => console.error(err));
    }
}
