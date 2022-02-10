import ReactDOM from 'react-dom';
import React, { createContext } from 'react';
import App from './App';
import MainStore from './MainStore';
import 'antd/dist/antd.less';
import './styles.less';

const store = new MainStore();
export const OrgStructContext = createContext(store);

ReactDOM.render(
    <OrgStructContext.Provider value={store}>
        <App />
    </OrgStructContext.Provider>,
    document.getElementById('root'),
);
