
import React, {Component} from 'react';
import {bindActionCreators} from 'redux';
import {connect} from 'react-redux';
import {selectEis} from '../../actions/actionselectEis';
import loadEisen from '../../actions/actionLoadEisen';

class SideBar extends Component {

    componentDidMount() {
        $('.ui.sidebar').sidebar('show');
        this.props.loadEisen();
    }

    renderList() {
        return this.props.eisen.map((eis) => {
            return (
                 <a className="item"
                    key={eis.id}
                    onClick={() => this.props.selectEis(eis)}>
                    <span><i className="sitemap icon"></i>{eis.title}</span>
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


export default connect(
    (state)=> { return { eisen: state.eisen }; }, 
    (dispatch) => { return bindActionCreators({selectEis: selectEis, loadEisen: loadEisen}, dispatch); })
        (SideBar);
