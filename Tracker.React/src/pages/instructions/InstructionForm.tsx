import React, { FC, useContext } from "react";
import { observer } from "mobx-react";
import { Form, Input, Button, TreeSelect, Select, DatePicker } from "antd";
import { StoreContext } from "../../App";

const { Option } = Select;
const width = "180px";

const InstructionForm: FC = observer(() => {
  const {
    instructionStore: store,
    instructionsStore,
    orgStructStore,
  } = useContext(StoreContext);
  const executors = orgStructStore.getChildren(store.creatorId!);

  return (
    <div>
      <h3>Создать новое поручение</h3>
      <Form layout={"inline"}>
        <Form.Item label="Текст поручения">
          <Input
            value={store.name}
            onChange={store.setName}
            className="instruction-form__field-input"
          />
        </Form.Item>
        <Form.Item label="Родительское поручение">
          <TreeSelect
            className="instruction-form__field-input"
            dropdownStyle={{ maxHeight: 400, overflow: "auto" }}
            treeData={instructionsStore.instructionsTreeData}
            treeDefaultExpandAll
            value={store.parentId}
            onChange={store.setParentId}
            allowClear
          />
        </Form.Item>
        <Form.Item label="Создатель">
          <TreeSelect
            className="instruction-form__field-input"
            dropdownStyle={{ maxHeight: 400, overflow: "auto" }}
            treeData={orgStructStore.orgStructTreeData}
            treeDefaultExpandAll
            value={store.creatorId}
            onChange={store.setCreatorId}
            allowClear
          />
        </Form.Item>
        <Form.Item label="Исполнитель">
          <Select
            value={store.executorId}
            onChange={store.setExecutorId}
            className="instruction-form__field-input"
          >
            {executors.map((e) => (
              <Option key={e.id}>{e.name}</Option>
            ))}
          </Select>
        </Form.Item>
        <Form.Item label="Дедлайн">
          <DatePicker
            className="instruction-form__field-input"
            value={store.deadline}
            onChange={store.setDeadline}
            format="DD.MM.YYYY"
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

export default InstructionForm;
