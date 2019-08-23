import React, {Component} from 'react';
import {Route} from 'react-router';
import {Layout} from './components/Layout';
import {ChatRoom} from "./components/ChatRoom";
import {Login} from "./components/Login";
import {Lobby} from "./components/Lobby";
import {Create} from "./components/Create";

export default class App extends Component {
    static displayName = App.name;

    render() {
        return (
            <Layout>
                <Route exact path='/' name="lobby" component={Lobby}/>
                <Route exact path='/login' name="login" component={Login}/>
                <Route exact path='/create' name="create" component={Create}/>
                <Route exact path='/chat' name="chat" component={ChatRoom}/>
            </Layout>
        );
    }
}
