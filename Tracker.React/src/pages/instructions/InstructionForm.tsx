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
import { StoreContext } from "./InstructionsPage";
import { userStore } from "../../auth/UserStore";
import moment, { Moment } from "moment";

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
        onCancel={store.hideModal}
        footer={[
          <Button key="back" onClick={store.hideModal}>
            Отмена
          </Button>,
          <Button
            key="submit"
            type="primary"
            onClick={async (): Promise<void> => {
              const wasSaved = await store.save();
              if (wasSaved) instructionsStore.load();
            }}
          >
            Сохранить
          </Button>,
        ]}
      >
        <Form layout={"vertical"}>
          <Form.Item
            label="Текст поручения"
            validateStatus={store.errors?.name && "error"}
            help={store.errors?.name}
          >
            <Input
              value={store.name}
              className="instruction-form__field-input"
              onChange={(e: React.ChangeEvent<HTMLInputElement>): void =>
                store.setName(e.target.value)
              }
            />
          </Form.Item>
          {store.parentId && (
            <Form.Item
              label="Родительское поручение"
              validateStatus={store.errors?.parentId && "error"}
              help={store.errors?.parentId}
            >
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
          <Form.Item
            label="Исполнитель"
            validateStatus={store.errors?.executorId && "error"}
            help={store.errors?.executorId}
          >
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
          <Form.Item
            label="Дедлайн"
            validateStatus={store.errors?.deadline && "error"}
            help={store.errors?.deadline}
          >
            <DatePicker
              className="instruction-form__field-input"
              value={store.deadline && moment(store.deadline)}
              onChange={(momentDate: Moment | null): void =>
                store.setDeadline(momentDate?.toDate())
              }
              format="DD.MM.YYYY"
            />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
});

export default InstructionForm;
