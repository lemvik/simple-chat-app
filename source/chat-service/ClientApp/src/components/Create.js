import React, {Component} from 'react';
import {Redirect} from "react-router-dom";

export class Create extends Component {
    static displayName = Create.name;

    constructor(props) {
        super(props);
        this.state = {login: "", password: "", token: "", alias: "", authorized: false};
        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleNameChange = this.handleNameChange.bind(this);
        this.handlePasswordChange = this.handlePasswordChange.bind(this);
        this.handleAliasChange = this.handleAliasChange.bind(this);
    }

    handleSubmit(event) {
        fetch('/auth/create', {
            method: 'post',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({
                'Name': this.state.login,
                'Password': this.state.password,
                'Alias': this.state.alias
            })
        }).then(response => response.json()).then(response => {
            this.setState({token: response.token, alias: response.alias, authorized: true})
        });

        event.preventDefault();
    }

    handleNameChange(event) {
        this.setState({login: event.target.value});
    }

    handlePasswordChange(event) {
        this.setState({password: event.target.value});
    }
    
    handleAliasChange(event) {
        this.setState({alias: event.target.value});
    }

    render() {
        if (this.state.authorized) {
            return (<Redirect to={ {pathname: '/chat', state: {alias: this.state.alias, token: this.state.token }} } />);
        }

        return (
            <form onSubmit={this.handleSubmit}>
                <div><label>Login:</label><input type="text" value={this.state.login} onChange={this.handleNameChange}/>
                </div>
                <div><label>Password:</label><input type="password" value={this.state.password}
                                                    onChange={this.handlePasswordChange}/></div>
                <div><label>Nickname:</label><input type="text" value={this.state.alias}
                                                    onChange={this.handleAliasChange}/></div>
                <input type="submit"/>
            </form>
        );
    }
}
