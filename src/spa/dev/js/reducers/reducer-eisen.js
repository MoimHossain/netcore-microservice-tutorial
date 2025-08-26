import * as ACTIONS from '../actions/actionTypes';

export default function (state=[], action) {
    switch(action.type) {
        case ACTIONS.GET_EISEN: 
           return action.payload;
        case ACTIONS.EISEN_UPDATED:
           return state.map(item => 
               item.id === action.payload.id ? action.payload : item
           );
        case ACTIONS.EISEN_ADDED:
           return [...state, action.payload];
        case ACTIONS.EISEN_DELETED:
           return state.filter(item => item.id !== action.payload);
        default:
           return state;
    }
}
