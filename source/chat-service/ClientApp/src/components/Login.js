import React, {Component} from 'react';
import {Redirect} from "react-router-dom";

export class Login extends Component {
    static displayName = Login.name;

    constructor(props) {
        super(props);
        this.state = {login: "", password: "", token: "", alias: "", authorized: false};
        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleNameChange = this.handleNameChange.bind(this);
        this.handlePasswordChange = this.handlePasswordChange.bind(this);
    }

    handleSubmit(event) {
        fetch('/auth/authenticate', {
            method: 'post',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({
                'Name': this.state.login,
                'Password': this.state.password
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

    render() {
        if (this.state.authorized) {
            return (<Redirect to={{pathname: '/chat', state: {alias: this.state.alias, token: this.state.token}}}/>);
        }

        return (
            <form onSubmit={this.handleSubmit}>
                <div className={'form-group'}>
                    <label>Login:</label>
                    <input type="text" value={this.state.login} className={'form-control'}
                           onChange={this.handleNameChange}
                           placeholder={"Login"}/>
                </div>
                <div className={'form-group'}>
                    <label>Password:</label>
                    <input type="password" value={this.state.password} className={'form-control'}
                           onChange={this.handlePasswordChange}
                           placeholder={"Password"}/>
                </div>
                <button type="submit" className="btn btn-primary">Submit</button>
            </form>
        );
    }
}
