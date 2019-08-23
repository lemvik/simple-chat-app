import React, {Component} from 'react';

export class ChatRoom extends Component {
    constructor(props) {
        super(props);
        this.state = {messages: [], input: ""};
        this.props.connection.on("ReceiveMessage", this.addMessage.bind(this));
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
        this.props.connection.invoke("SendMessage", "me", this.state.input).catch(err => console.error(err));
        event.preventDefault();
    }

    render() {
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
