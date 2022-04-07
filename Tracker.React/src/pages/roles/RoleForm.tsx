import React, { FC, useState } from "react";
import { observer } from "mobx-react";
import { Form, Input, Button, Modal } from "antd";
import { RolesStore } from "./RolesStore";
import { RoleStore } from "./RoleStore";

interface Props {
  rolesStore: RolesStore;
  roleStore: RoleStore;
}

const RoleForm: FC<Props> = observer((props: Props) => {
  const { rolesStore, roleStore: store } = props;

  return (
    <div>
      <Button type="primary" onClick={(): void => store.showModal()}>
        Создать новую роль
      </Button>
      <Modal
        title={store.isEditMode ? "Редактирование роли" : "Создание новой роли"}
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
              if (wasSaved) rolesStore.load();
            }}
          >
            Сохранить
          </Button>,
        ]}
      >
        <Form layout={"vertical"} autoComplete="off">
          <Form.Item
            label="Название роли"
            validateStatus={store.errors?.name && "error"}
            help={store.errors?.name}
          >
            <Input
              value={store.name}
              onChange={(e: React.ChangeEvent<HTMLInputElement>): void =>
                store.setName(e.target.value)
              }
            />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
});

export default RoleForm;
