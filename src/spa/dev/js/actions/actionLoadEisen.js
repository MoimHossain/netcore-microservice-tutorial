import * as ACTIONS from './actionTypes';
import axios from 'axios';

// The default data when the API is unreachable
var ___x = [
        {
            id: '1',
            title: 'Hyper-loop',
            modelImage: 'http://www.stonybrook.edu/happenings/wp-content/uploads/Hyperloop-interior.jpg',
            description: "Build hyper-loop tunnel from Amsterdam to Madrid",
            phase: "Initial ontwerp"
        }
    ];

function getAllEisenAsync(data) {
    return {
        type: ACTIONS.GET_EISEN,
        payload: data
    };
}

export function eisenUpdated(eisenItem) {
    return {
        type: ACTIONS.EISEN_UPDATED,
        payload: eisenItem
    };
}

export function eisenAdded(eisenItem) {
    return {
        type: ACTIONS.EISEN_ADDED,
        payload: eisenItem
    };
}

export function eisenDeleted(eisenId) {
    return {
        type: ACTIONS.EISEN_DELETED,
        payload: eisenId
    };
}

export default function getAllEisen() {
    return dispatch => {        
        axios.get('/api/eisen')
             .then(res => {                                  
                 dispatch(getAllEisenAsync(res.data));
             }, error => {                 
                 dispatch(getAllEisenAsync(___x));
             });
    }
}