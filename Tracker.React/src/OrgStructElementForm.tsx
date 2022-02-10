import React, { FC, useContext  } from 'react';
import { observer } from 'mobx-react';
import { OrgStructContext } from '.';
import { Form, Input, Button, TreeSelect } from 'antd';

const OrgStructElementForm: FC = observer(() => {
    const store = useContext(OrgStructContext);
    
    return (
        <div>
            <h3>Создать нового сотрудника</h3>
            <Form layout={'inline'} >
                <Form.Item label="ФИО" >
                    <Input 
                        value={store.orgStructElementStore.name}
                        onChange={store.orgStructElementStore.setName} 
                        style={{ width: '300px' }} 
                    />
                </Form.Item>
                <Form.Item label="Руководитель">
                    <TreeSelect
                        style={{ width: '300px' }}
                        dropdownStyle={{ maxHeight: 400, overflow: 'auto' }}
                        treeData={store.orgStructStore.orgStructTreeData}
                        treeDefaultExpandAll
                        value={store.orgStructElementStore.parentId}
                        onChange={store.orgStructElementStore.setParentId}
                        allowClear
                    />
                </Form.Item>
                <Form.Item>
                    <Button type="primary" onClick={store.save}>Сохранить</Button>
                </Form.Item>
            </Form>
        </div>
    );
});

export default OrgStructElementForm;