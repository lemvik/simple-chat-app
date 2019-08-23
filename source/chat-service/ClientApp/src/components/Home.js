import React, {Component} from 'react';
import {HubConnectionBuilder} from '@aspnet/signalr';
import {ChatRoom} from './ChatRoom';

export class Home extends Component {
    static displayName = Home.name;

    constructor(props) {
        super(props);
        this.state = {connected: false};
        this.connection = new HubConnectionBuilder().withUrl("/chat").build();

        this.connection.start().then(function () {
            this.setState(() => {
                return {connected: true};
            })
        }.bind(this)).catch(function (error) {
            return console.error(error.toString());
        });
    }

    render() {
        if (!this.state.connected) {
            return (<p><em>Connecting...</em></p>);      
        }
        
        return (
            <div>
                <ChatRoom connection={this.connection}/>
            </div>
        );
    }
}
