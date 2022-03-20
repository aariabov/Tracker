import React, { FC, useContext } from "react";
import { observer } from "mobx-react";
import {
  Form,
  Input,
  Button,
  TreeSelect,
  Select,
  DatePicker,
  Modal,
} from "antd";
import { StoreContext } from "../../App";
import { userStore } from "../../auth/UserStore";

const { Option } = Select;

const InstructionForm: FC = observer(() => {
  const {
    instructionStore: store,
    instructionsStore,
    orgStructStore,
  } = useContext(StoreContext);
  const executors = orgStructStore.getChildren(userStore.id);

  return (
    <div>
      <Button type="primary" onClick={store.showModal}>
        Создать новое поручение
      </Button>
      <Modal
        title="Создание нового поручения"
        visible={store.isModalVisible}
        onOk={store.save}
        onCancel={store.hideModal}
        footer={[
          <Button key="back" onClick={store.hideModal}>
            Отмена
          </Button>,
          <Button key="submit" type="primary" onClick={store.save}>
            Сохранить
          </Button>,
        ]}
      >
        <Form layout={"vertical"}>
          <Form.Item label="Текст поручения">
            <Input
              value={store.name}
              onChange={store.setName}
              className="instruction-form__field-input"
            />
          </Form.Item>
          {store.parentId && (
            <Form.Item label="Родительское поручение">
              <TreeSelect
                className="instruction-form__field-input"
                dropdownStyle={{ maxHeight: 400, overflow: "auto" }}
                treeData={instructionsStore.instructionsTreeData}
                treeDefaultExpandAll
                value={store.parentId}
                onChange={store.setParentId}
                allowClear
                disabled
              />
            </Form.Item>
          )}
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
        </Form>
      </Modal>
    </div>
  );
});

export default InstructionForm;
