import { Button, Card, Col, Form, Input, Row, Space, Typography } from "antd";
import Title from "antd/lib/typography/Title";
import { observer } from "mobx-react";
import React, { FC, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { errorStore } from "../stores/ErrorStore";
import { LoginStore } from "./LoginStore";

const { Text } = Typography;

const Login: FC = observer(() => {
  const [loginStore] = useState(() => new LoginStore());
  const navigate = useNavigate();
  const location = useLocation();

  let state = location.state as { from: Location };
  let from = state ? state.from.pathname : "/";

  const submitFrom = async (): Promise<void> => {
    loginStore.clearError();
    await loginStore.handleSubmit();
    if (errorStore.hasError) {
      errorStore.clearError();
    }
    navigate(from, { replace: true });
  };

  return (
    <Row justify="center" align="middle" className="login">
      <Col>
        <Card>
          <Form layout="vertical" className="login__form" onFinish={submitFrom}>
            <Title level={3} className="login__title">
              Вход
            </Title>
            <Form.Item label="Email" name="email">
              <Input name="email" onChange={loginStore.setEmail} />
            </Form.Item>
            <Form.Item label="Пароль" name="password">
              <Input
                type="password"
                name="password"
                onChange={loginStore.setPassword}
              />
            </Form.Item>
            <Space direction="vertical">
              {loginStore.hasError && (
                <Text type="danger">{loginStore.error}</Text>
              )}
              <Button
                type="primary"
                htmlType="submit"
                className="login__btn"
                block
              >
                Войти
              </Button>
            </Space>
          </Form>
        </Card>
      </Col>
    </Row>
  );
});

export default Login;
