import React, { FC } from 'react';
import { observer } from 'mobx-react';
import OrgStruct from './OrgStruct';
import OrgStructElementForm from './OrgStructElementForm';
import { Space } from 'antd';

const App: FC = observer(() => {
    return <div className='app'>
        <Space direction="vertical" size="large">
            <OrgStructElementForm />
            <OrgStruct />
        </Space>
    </div>;
});

export default App;
