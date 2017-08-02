
import React from 'react';
import UserList from '../containers/user-list';
import UserDetails from '../containers/user-detail';
import SideBar from '../containers/layouts/side-bar';
require('../../scss/style.scss');

const App = () => (
    <div>
        <SideBar />
          <div className="pusher">
                <h2>User List</h2>
                <UserList />
                <hr />
                <h2>User Details</h2>
                <UserDetails />
          </div>
        
    </div>
);

export default App;
