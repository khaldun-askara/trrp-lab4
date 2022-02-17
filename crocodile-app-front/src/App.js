import React, { useState, useRef, useEffect, useCallback } from "react";
import Message from "./components/message";
import Form from "./components/form";
import Croc_canvas from "./components/croc_canvas";
import Suggestions from "./components/suggestions";
import CanvasDraw from "react-canvas-draw";
import configData from "./Config.json";

class App extends React.Component {

  componentDidMount() {
    // this is an "echo" websocket service


    // подключаемся к серверу
    this.connection = new WebSocket('ws://' + configData.url);
    this.connection.onclose = () => {
      this.setState({ messages: this.messages.push(<Message message={"Disconnected... Проверьте подключение к интернету или повторите попытку позже"} sender="" />) });
    }
    // this.connection.onopen = () => {
    //   this.setState({ messages: this.messages.push(<Message message={"Подключение восстановлено"} sender="" />) });
    // }
    // listen to onmessage event
    this.connection.onmessage = async (evt) => {
      var reply = JSON.parse(evt.data);
      switch (reply.type) {
        // при подключении получаем роль
        case 'drawing_role':
          if (reply.message === 'true')
            this.role = 'Рисующий';
          else
            this.role = 'Угадывающий';
          console.log(this.role);

          this.setState({ messages: this.messages.push(<Message message={"Ваша роль: " + this.role} sender="" />) });
          break;
        // рисующий получает слово для рисования
        case 'word':
          console.log(reply.message);
          this.setState({ messages: this.messages.push(<Message message={"Слово для рисования: " + reply.message} sender="" />) });
          this.canvas.clear();
          break;
        // получаем сообщение в чат
        case 'chat':
          var chat_message = JSON.parse(reply.message);
          this.setState({
            messages: this.messages.push(<Message message={chat_message.message} sender={chat_message.nickname} />)
          });
          break;
        // обновляем рисунок
        case 'lineart':
          console.log(reply.message);
          // this.canvas.loadSaveData(reply.message)
          // this.setState({ 
          //   messages: this.messages.push(<Message message={reply.type + ': ' + reply.message} sender="" />) 
          // });
          this.canvas.loadSaveData(reply.message, true);
          break;

        default:
          this.setState({
            messages: this.messages.push(<Message message={reply.type + ': ' + reply.message} sender="" />)
          });
          break;
      }

    };
  }
// список сообщений в чате
  messages = []
// отправить сообщение
  sendMessage = async (e) => {
    e.preventDefault();
    const text = e.target.elements.message.value;
    e.target.reset();

    let WSMessage = {
      type: 'chat',
      message: text
    };

    var a = JSON.stringify(WSMessage);

    this.connection.send(a);
  }
// после нового сообщения пролистываем чат вниз
  scrollToBottom() {
    const scrollHeight = this.messageList.scrollHeight;
    const height = this.messageList.clientHeight;
    const maxScrollTop = scrollHeight - height;
    this.messageList.scrollTop = maxScrollTop > 0 ? maxScrollTop : 0;
  }

  componentDidUpdate() {
    this.scrollToBottom();
    // this.sendLineArt();
  }
// отправляем измененный рисунок
  sendLineArt = async (e) => {
    // e.preventDefault();

    if (this.role == 'Рисующий') {
      let WSMessage = {
        type: 'lineart',
        message: this.canvas.getSaveData()
      };

      var a = JSON.stringify(WSMessage);

      console.log(a);

      this.connection.send(a);
    }
  }
// отправляем предложенное слово
  sendSuggestion = async (e) => {
    e.preventDefault();
    const word = e.target.elements.message.value;
    e.target.reset();

    let WSMessage = {
      type: 'suggestion',
      message: word
    };

    var a = JSON.stringify(WSMessage);

    console.log(a)

    this.connection.send(a);
  }
// отменяем изменение в рисунке
  UndoLine = async (e) => {
    e.preventDefault();
    this.canvas.undo();
  }

  render() {
    return (
      <div className="wrapper">
        <form className="drawing-space" onSubmit={this.UndoLine}>
          <CanvasDraw
            onChange={this.sendLineArt}
            ref={canvasDraw => (this.canvas = canvasDraw)}
            brushRadius={5}
            disabled={this.role == 'Угадывающий'}
            className="canvas" />
          {this.role == 'Рисующий' && <button>Отменить</button>}
        </form>
        <div>
          <div className="chat" ref={(div) => { this.messageList = div; }}>
            {this.messages}
          </div>
          {this.role == 'Угадывающий' && <Form sendMessage={this.sendMessage} />}
          <Suggestions sendSuggestion={this.sendSuggestion} />
        </div>
      </div>
    );
  }
}

export default App