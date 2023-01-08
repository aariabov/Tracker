import React, { FC } from "react";
import { observer } from "mobx-react";
import { Form, Input, Button, TreeSelect, Modal, Select } from "antd";
import { OrgStructStore } from "../../stores/OrgStructStore";
import { OrgStructElementStore } from "./OrgStructElementStore";
import { RolesStore } from "../roles/RolesStore";

interface Props {
  orgStructStore: OrgStructStore;
  orgStructElementStore: OrgStructElementStore;
  rolesStore: RolesStore;
}

const OrgStructElementForm: FC<Props> = observer((props: Props) => {
  const { orgStructStore, rolesStore, orgStructElementStore: store } = props;

  return (
    <div>
      <Button type="primary" onClick={(): void => store.showModal()}>
        Создать нового сотрудника
      </Button>
      <Modal
        title={
          store.isEditMode
            ? "Редактирование сотрудника"
            : "Создание нового сотрудника"
        }
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
              if (wasSaved) orgStructStore.load();
            }}
          >
            Сохранить
          </Button>,
        ]}
      >
        <Form layout={"vertical"} autoComplete="off">
          <Form.Item
            label="ФИО"
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
          <Form.Item
            label="Email"
            validateStatus={store.errors?.email && "error"}
            help={store.errors?.email}
          >
            <Input
              value={store.email}
              onChange={(e: React.ChangeEvent<HTMLInputElement>): void =>
                store.setEmail(e.target.value)
              }
            />
          </Form.Item>
          {!store.isEditMode && (
            <Form.Item
              label="Пароль"
              validateStatus={store.errors?.password && "error"}
              help={store.errors?.password}
            >
              <Input
                value={store.password}
                onChange={(e: React.ChangeEvent<HTMLInputElement>): void =>
                  store.setPassword(e.target.value)
                }
              />
            </Form.Item>
          )}
          <Form.Item
            label="Руководитель"
            validateStatus={store.errors?.bossId && "error"}
            help={store.errors?.bossId}
          >
            <TreeSelect
              dropdownStyle={{ maxHeight: 400, overflow: "auto" }}
              treeData={orgStructStore.orgStructTreeData}
              treeDefaultExpandAll
              value={store.parentId ?? undefined}
              onChange={store.setParentId}
              allowClear
            />
          </Form.Item>
          <Form.Item
            label="Роли"
            validateStatus={store.errors?.roles && "error"}
            help={store.errors?.roles}
          >
            <Select
              mode="multiple"
              allowClear
              value={store.roles}
              onChange={store.setRoles}
              options={rolesStore.roles.map((r) => ({
                value: r.name,
                label: r.name,
              }))}
            />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
});

export default OrgStructElementForm;
