
import React, {Component} from 'react';
import {bindActionCreators} from 'redux';
import {connect} from 'react-redux';
import {selectEis} from '../../actions/actionselectEis'


class SideBar extends Component {

    componentDidMount() {
        //$('.ui.sidebar').sidebar('toggle');
    }

    renderList() {
        return this.props.eisen.map((eis) => {
            return (
                 <a className="item"
                    key={eis.id}
                    onClick={() => this.props.selectEis(eis)}>
                    {eis.description} {eis.phase}
                </a>
            );
        });
    }

    render() {
        return (
            <div className="ui visible sidebar inverted vertical menu">
                {this.renderList()}
            </div>
        );
    }

}


function mapStateToProps(state) {
    return {
        eisen: state.eisen
    };
}

function matchDispatchToProps(dispatch){
    return bindActionCreators({selectEis: selectEis}, dispatch);
}

export default connect(mapStateToProps, matchDispatchToProps)(SideBar);
