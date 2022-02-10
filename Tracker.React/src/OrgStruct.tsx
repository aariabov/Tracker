import React, { FC, useContext } from 'react';
import { observer } from 'mobx-react';
import { Tree } from 'antd';
import { OrgStructContext } from '.';

const OrgStruct: FC = observer(() => {
    const { orgStructStore } = useContext(OrgStructContext);
    
    return (
        <div>
            <h3>Сотрудники ({orgStructStore.orgStructElements.length})</h3>
            <Tree
                key={Math.random()}
                showLine={{ showLeafIcon: false }}
                defaultExpandAll
                showIcon={false}
                treeData={orgStructStore.orgStructTreeData}
            />
        </div>
    );
});

export default OrgStruct;
