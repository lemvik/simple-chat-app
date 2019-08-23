import React, {Component} from 'react';
import {Link} from "react-router-dom";

export class Lobby extends Component {
    static displayName = Lobby.name;

    constructor(props) {
        super(props);
        this.state = {connected: false};
    }

    render() {
        return (
            <div>
                <p>Create a new player or log in as an existing one.</p>
                <div>
                    <ul>
                        <li>
                            <Link to="login">Login</Link>
                        </li>
                        <li>
                            <Link to="create">Create</Link>
                        </li>
                    </ul>
                </div>
            </div>
        );
    }
}
