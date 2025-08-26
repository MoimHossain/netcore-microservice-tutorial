import React, { Component } from 'react';
import { connect } from 'react-redux';
import SideBar from '../containers/layouts/side-bar';
import EisDetails from '../containers/eis-details';
import signalRService from '../services/signalRService';
require('../../scss/style.scss');

class AppContainer extends Component {
    componentDidMount() {
        // Initialize SignalR connection
        signalRService.startConnection(this.props.dispatch);
    }

    componentWillUnmount() {
        // Clean up SignalR connection
        signalRService.stopConnection();
    }

    render() {
        return (
            <div>
                <SideBar />
                <div className="pusher">
                    <EisDetails/>
                    <div className="signalr-status">
                        SignalR: {signalRService.isConnected() ? 'Connected' : 'Disconnected'}
                    </div>
                </div>
            </div>
        );
    }
}

export default connect()(AppContainer);