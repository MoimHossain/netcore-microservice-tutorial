
import React, {Component} from 'react';
import {connect} from 'react-redux';

class EisDetail extends Component {
    render() {
        if (!this.props.eis) {            
            return (
                <div>
                    No eis selected.
                </div>
            );
        }
        
        return (            
            <div className="ui ordered steps">
              <div className="completed step">
                <div className="content">
                  <div className="title">SO</div>
                  <div className="description">Schets ontwerp</div>
                </div>
              </div>
              <div className="completed step">
                <div className="content">
                  <div className="title">DO</div>
                  <div className="description">Definitive ontwerp</div>
                </div>
              </div>
              <div className="active step">
                <div className="content">
                  <div className="title">VO</div>
                  <div className="description">Voorlopig ontwerp</div>
                </div>
              </div>  
              <div className="active step">
                <div className="content">
                  <div className="title">UO</div>
                  <div className="description">U? ontwerp</div>
                </div>
              </div>
            </div>
        );
    }
}

function mapStateToProps(state) {
    return {
        eis: state.activeEis
    };
}

export default connect(mapStateToProps)(EisDetail);

/*<img src={this.props.user.thumbnail} />
                <h2>{this.props.user.first} {this.props.user.last}</h2>
                <h3>Age: {this.props.user.age}</h3>
                <h3>Description: {this.props.user.description}</h3> */