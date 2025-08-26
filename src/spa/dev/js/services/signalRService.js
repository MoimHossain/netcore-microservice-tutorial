import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { eisenUpdated, eisenAdded, eisenDeleted } from '../actions/actionLoadEisen';

class SignalRService {
    constructor() {
        this.connection = null;
        this.dispatch = null;
    }

    async startConnection(dispatch) {
        this.dispatch = dispatch;
        
        this.connection = new HubConnectionBuilder()
            .withUrl('/eisenhub')
            .configureLogging(LogLevel.Information)
            .build();

        this.connection.on('EisenUpdated', (eisenItem) => {
            console.log('Eisen updated:', eisenItem);
            if (this.dispatch) {
                this.dispatch(eisenUpdated(eisenItem));
            }
        });

        this.connection.on('EisenAdded', (eisenItem) => {
            console.log('Eisen added:', eisenItem);
            if (this.dispatch) {
                this.dispatch(eisenAdded(eisenItem));
            }
        });

        this.connection.on('EisenDeleted', (eisenId) => {
            console.log('Eisen deleted:', eisenId);
            if (this.dispatch) {
                this.dispatch(eisenDeleted(eisenId));
            }
        });

        try {
            await this.connection.start();
            console.log('SignalR Connected.');
        } catch (err) {
            console.log('SignalR Connection Error: ', err);
        }
    }

    async stopConnection() {
        if (this.connection) {
            await this.connection.stop();
            console.log('SignalR Disconnected.');
        }
    }

    async sendMessage(user, message) {
        if (this.connection) {
            try {
                await this.connection.invoke('SendMessage', user, message);
            } catch (err) {
                console.error('Error sending message: ', err);
            }
        }
    }

    isConnected() {
        return this.connection && this.connection.state === 'Connected';
    }
}

export default new SignalRService();