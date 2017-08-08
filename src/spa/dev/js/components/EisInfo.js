
import React from 'react';

const EisInfo = (props) => (
    <div>
        <h1 className="ui header">{props.eis.title}</h1>
        <span>{props.eis.description}</span>
        <div className="ignored info ui message">
            HI MOIM Expected to be completed in a month. Currently is on <b>{props.eis.phase}</b>
        </div>        
        <div>
            <a href="#" className="ui large rounded image">
                <img src={props.eis.modelImage}></img>
            </a>
        </div>
        <div className="ui stacked segment">
            <h3 className="ui header">Related Eisen</h3>
            <div className="ui list">
                <div className="item">H1 34 NSHKK :L 1/.0 SIS</div>
                <div className="item">H3 44 ADNSHKK :L 1/.0 SIS</div>
                <div className="item">H3 44 ADNSHKK :L 1/.0 SIS</div>
            </div>
        </div>
    </div>
);

export default EisInfo;
