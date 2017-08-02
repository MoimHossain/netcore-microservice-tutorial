
import React from 'react';
import UserList from '../containers/user-list';
import UserDetails from '../containers/user-detail';
import SideBar from '../containers/layouts/side-bar';
import EisDetails from '../containers/eis-details';
require('../../scss/style.scss');

const App = () => (
    <div>
        <SideBar />
        <div className="pusher">
            <EisDetails/>
        </div>
    </div>
);

export default App;
