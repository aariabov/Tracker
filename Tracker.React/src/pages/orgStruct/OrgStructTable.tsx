import React, { FC, ReactNode } from "react";
import { observer } from "mobx-react";
import { Modal, Table } from "antd";
import {
  OrgStructElementRow,
  OrgStructStore,
} from "../../stores/OrgStructStore";
import { OrgStructElementStore } from "./OrgStructElementStore";

interface Props {
  orgStructStore: OrgStructStore;
  orgStructElementStore: OrgStructElementStore;
}

const OrgStructTable: FC<Props> = observer((props: Props) => {
  const { orgStructStore: store, orgStructElementStore } = props;
  const dataWasLoaded = store.orgStructElements.length > 0;

  const showConfirmModal = (user: OrgStructElementRow): void => {
    Modal.confirm({
      title: "Вы действительно хотите удалить пользователя?",
      content: `Пользователь "${user.name}" будет удален!`,
      async onOk() {
        const result = await orgStructElementStore.deleteUser(user.id);
        if (result?.modelErrors) {
          Modal.error({
            title: "Произошла ошибка при удалении пользователя!",
            content: result.modelErrors.id,
          });
        } else {
          store.load();
        }
      },
    });
  };

  const editOrgStructElement = (user: OrgStructElementRow): void => {
    orgStructElementStore.showModal(user);
  };

  return (
    <div>
      <h3>Сотрудники ({store.orgStructElements.length})</h3>
      {dataWasLoaded ? (
        <Table<OrgStructElementRow>
          key={Math.random()}
          dataSource={store.orgStructElementRows}
          rowKey="id"
          defaultExpandAllRows
        >
          <Table.Column<OrgStructElementRow>
            title="Название"
            dataIndex="name"
          />
          <Table.Column<OrgStructElementRow> title="Email" dataIndex="email" />
          <Table.Column<OrgStructElementRow>
            title="Роли"
            dataIndex="roles"
            width="20%"
            render={(_: string, record: OrgStructElementRow): ReactNode =>
              record.roles.join(", ")
            }
          />
          <Table.Column<OrgStructElementRow>
            width="10%"
            render={(_: string, record: OrgStructElementRow): ReactNode => (
              <a onClick={(): void => editOrgStructElement(record)}>
                Редактировать
              </a>
            )}
          />
          <Table.Column<OrgStructElementRow>
            width="10%"
            render={(_: string, record: OrgStructElementRow): ReactNode => (
              <a onClick={(): void => showConfirmModal(record)}>Удалить</a>
            )}
          />
        </Table>
      ) : (
        "loading tree"
      )}
    </div>
  );
});

export default OrgStructTable;
