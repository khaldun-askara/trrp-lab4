import React, { useState, useRef, useEffect, useCallback } from "react";
import Message from "./components/message";
import Form from "./components/form";
import Croc_canvas from "./components/croc_canvas";
import Suggestions from "./components/suggestions";
import CanvasDraw from "react-canvas-draw";

class App extends React.Component {

  componentDidMount(){
    // this is an "echo" websocket service
    this.connection = new WebSocket('ws://127.0.0.1');
    // listen to onmessage event
    this.connection.onmessage = async (evt) => { 
      var reply = JSON.parse(evt.data);
        switch(reply.type) {
          case 'drawing_role':
            var role;
            if (reply.message === 'true')
              role = 'Рисующий';
            else
              role = 'Угадывающий';
            console.log(role);

            this.setState({ messages: this.messages.push(<Message message={"Ваша роль: " + role} sender="" />) });
            break;

          case 'word':
            console.log(reply.message);
            this.setState({ messages: this.messages.push(<Message message={"Слово для рисования: " + reply.message} sender="" />) });
            break;

          case 'chat':
            var chat_message = JSON.parse(reply.message);
            this.setState({ 
              messages: this.messages.push(<Message message={chat_message.message} sender={chat_message.nickname} />) 
            });
            break;

          case 'lineart':
            console.log(reply.message)
            // this.setState({ canvas: this.canvas.loadSaveData(reply.message)});
            this.setState({ 
              messages: this.messages.push(<Message message={reply.type + ': ' + reply.message} sender="" />) 
            });
            break;

          default:
            this.setState({ 
              messages: this.messages.push(<Message message={reply.type + ': ' + reply.message} sender="" />) 
            });
            break;
        }

    };
  }

  messages = []

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

  handleKeyPress = (event) => {
    if (event.ctrlKey && event.key === 'z') {
      this.setState({ messages: this.messages.push(<Message message="sdf" sender="" />) })
    }
  }

  sendLineArt = () => {
    // e.preventDefault();

    let WSMessage = {
      type: 'lineart',
      message: this.canvas.getSaveData()
    };

    var a = JSON.stringify(WSMessage);

    this.connection.send(a);
  }
  
  render() {
    return (
      <div className="wrapper">
        <form className="drawing-space">
          <CanvasDraw ref={(div) => { this.canvas = div; }} brushRadius={5} className="canvas" />
        </form>
        <div>
          <div className="chat" ref={(div) => { this.messageList = div; }}>
            {this.messages}
          </div>
          <Form sendMessage={this.sendMessage}/>
          <Suggestions />
        </div>
      </div>
    );
  }
}

export default App