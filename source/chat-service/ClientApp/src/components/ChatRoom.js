import React, {Component} from 'react';
import {HubConnectionBuilder} from '@aspnet/signalr';

export class ChatRoom extends Component {
    constructor(props) {
        super(props);
        this.state = {messages: [], input: "", connected: false};
        this.alias = props.location.state.alias;
        
        let token = props.location.state.token;
        
        console.log(this.alias);
        console.log(token);

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
        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    addMessage(user, message) {
        this.setState((state) => {
            return {messages: [...state.messages, {user: user, message: message}]};
        })
    }

    handleChange(event) {
        this.setState({input: event.target.value})
    }

    handleSubmit(event) {
        this.connection.invoke("SendMessage", this.props.alias, this.state.input).catch(err => console.error(err));
        event.preventDefault();
    }

    render() {
        if (!this.state.connected) {
            return (<p><em>Connecting...</em></p>);
        }
        
        let contents = this.state.messages.map(msg => <li key={msg.message}>
            <span>{msg.user}</span>: <span>{msg.message}</span></li>);

        return (
            <div>
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
}
