import React, {Component} from 'react';
import {Redirect} from "react-router-dom";

export class Create extends Component {
    static displayName = Create.name;

    constructor(props) {
        super(props);
        this.state = {login: "", password: "", token: "", alias: "", authorized: false, formError: null};
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
        this.setState({login: event.target.value, formError: null});
    }

    handlePasswordChange(event) {
        this.setState({password: event.target.value, formError: null});
    }

    handleAliasChange(event) {
        this.setState({alias: event.target.value, formError: null});
    }

    passwordState() {
        let passLen = this.state.password.length;
        let incorrectPass = passLen > 0 && passLen < 6;
        let validationError = this.state.formError != null;
        return {
            class: (incorrectPass || validationError) ? 'form-control is-invalid' : 'form-control',
            error: incorrectPass ? 'Password should be 6 symbols or more' : null
        };
    }

    loginState() {
        let validationError = this.state.formError != null;
        return {
            class: validationError ? 'form-control is-invalid' : 'form-control',
            error: validationError ? this.state.formError.toString() : null
        };
    }

    aliasState() {
        let validationError = this.state.formError != null;
        return {
            class: validationError ? 'form-control is-invalid' : 'form-control',
            error: validationError ? this.state.formError.toString() : null
        };
    }

    render() {
        if (this.state.authorized) {
            return (<Redirect to={{pathname: '/chat', state: {alias: this.state.alias, token: this.state.token}}}/>);
        }

        let loginSt = this.loginState();
        let passSt = this.passwordState();
        let aliasSt = this.aliasState();

        let loginMessage = loginSt.error ? (<div className="invalid-feedback">{loginSt.error}</div>) : "";
        let passMessage = passSt.error ? (<div className="invalid-feedback">{passSt.error}</div>) : "";
        let aliasMessage = aliasSt.error ? (<div className="invalid-feedback">{aliasSt.error}</div>) : "";

        return (
            <form onSubmit={this.handleSubmit}>
                <div className={'form-group'}>
                    <label>Login:</label>
                    <input type="text"
                           value={this.state.login}
                           className={loginSt.class}
                           onChange={this.handleNameChange}
                           placeholder={"Login"}/>
                    {loginMessage}
                </div>
                <div className={'form-group'}>
                    <label>Password:</label>
                    <input type="password"
                           value={this.state.password}
                           className={passSt.class}
                           onChange={this.handlePasswordChange}
                           placeholder={"Password"}/>
                    {passMessage}
                </div>
                <div className={'form-group'}>
                    <label>Nickname:</label>
                    <input type="text"
                           value={this.state.alias}
                           className={aliasSt.class}
                           onChange={this.handleAliasChange}
                           placeholder={'Nickname'}/>
                    {aliasMessage}
                </div>
                <button type="submit" className="btn btn-primary">Submit</button>
            </form>
        );
    }
}
