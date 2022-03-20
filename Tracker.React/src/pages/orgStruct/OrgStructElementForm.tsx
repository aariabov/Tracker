import React, { FC, useContext } from "react";
import { observer } from "mobx-react";
import { Form, Input, Button, TreeSelect } from "antd";
import { StoreContext } from "../../App";

const OrgStructElementForm: FC = observer(() => {
  const { orgStructElementStore: store, orgStructStore } =
    useContext(StoreContext);

  return (
    <div>
      <h3>Создать нового сотрудника</h3>
      <Form layout={"inline"}>
        <Form.Item label="ФИО">
          <Input
            value={store.name}
            onChange={(e: React.ChangeEvent<HTMLInputElement>): void =>
              store.setName(e.target.value)
            }
            style={{ width: "300px" }}
          />
        </Form.Item>
        <Form.Item label="Руководитель">
          <TreeSelect
            style={{ width: "300px" }}
            dropdownStyle={{ maxHeight: 400, overflow: "auto" }}
            treeData={orgStructStore.orgStructTreeData}
            treeDefaultExpandAll
            value={store.parentId}
            onChange={store.setParentId}
            allowClear
          />
        </Form.Item>
        <Form.Item>
          <Button type="primary" onClick={store.save}>
            Сохранить
          </Button>
        </Form.Item>
      </Form>
    </div>
  );
});

export default OrgStructElementForm;
