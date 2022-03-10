import { Button, Card, Col, Form, Input, Row } from "antd";
import Title from "antd/lib/typography/Title";
import { observer } from "mobx-react";
import React, { FC, useState } from "react";
import { LoginStore } from "./LoginStore";

const Login: FC = observer(() => {
  const [loginStore] = useState(() => new LoginStore());

  return (
    <Row justify="center" align="middle" className="login">
      <Col>
        <Card>
          <Form
            layout="vertical"
            autoComplete="off"
            className="login__form"
            onFinish={loginStore.handleSubmit}
          >
            <Title level={3} className="login__title">
              Вход
            </Title>
            <Form.Item label="Email" name="email">
              <Input
                autoComplete="off"
                name="email"
                onChange={loginStore.setEmail}
              />
            </Form.Item>
            <Form.Item label="Пароль" name="password">
              <Input
                type="password"
                autoComplete="off"
                name="password"
                onChange={loginStore.setPassword}
              />
            </Form.Item>
            <Button
              type="primary"
              htmlType="submit"
              className="login__btn"
              block
            >
              Войти
            </Button>
          </Form>
        </Card>
      </Col>
    </Row>
  );
});

export default Login;
