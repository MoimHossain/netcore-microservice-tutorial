import {combineReducers} from 'redux';
import EisenReducer from './reducer-eisen';
import ActiveEisReducer from './reducer-active-eis';

/*
 * We combine all reducers into a single object before updated data is dispatched (sent) to store
 * Your entire applications state (store) is just whatever gets returned from all your reducers
 * */

const allReducers = combineReducers({
    eisen: EisenReducer,
    activeEis: ActiveEisReducer
});

export default allReducers
