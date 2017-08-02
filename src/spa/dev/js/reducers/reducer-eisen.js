
export default function (state=[], action) {    
    
    switch(action.type) {
        case 'GET_EISEN': 
           return action.payload;
        default:
           return state;
    }
    return state;
}
