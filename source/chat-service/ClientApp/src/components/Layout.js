import React, {Component} from 'react';
import {Container} from 'reactstrap';

export class Layout extends Component {
    static displayName = Layout.name;

    render() {
        return (
            <Container fluid={true}>
                {this.props.children}
            </Container>
        );
    }
}
