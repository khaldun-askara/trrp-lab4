import React, { useState, useRef, useEffect, useCallback } from "react";
import Message from "./components/message";
import Form from "./components/form";
import Croc_canvas from "./components/croc_canvas";
import Suggestions from "./components/suggestions";
import CanvasDraw from "react-canvas-draw";
import configData from "./Config.json";
import { TouchScaleState } from "react-canvas-draw/lib/interactionStateMachine";

class App extends React.Component {

  componentDidMount(){
    // this is an "echo" websocket service

    

    this.connection = new WebSocket('ws://' + configData.url);
    // listen to onmessage event
    this.connection.onmessage = async (evt) => { 
      var reply = JSON.parse(evt.data);
        switch(reply.type) {
          case 'drawing_role':
            
            if (reply.message === 'true')
              this.role = 'Рисующий';
            else
              this.role = 'Угадывающий';
            console.log(this.role);

            this.setState({ messages: this.messages.push(<Message message={"Ваша роль: " + this.role} sender="" />) });
            break;

          case 'word':
            console.log(reply.message);
            this.setState({ messages: this.messages.push(<Message message={"Слово для рисования: " + reply.message} sender="" />) });
            this.canvas.clear();
            break;

          case 'chat':
            var chat_message = JSON.parse(reply.message);
            this.setState({ 
              messages: this.messages.push(<Message message={chat_message.message} sender={chat_message.nickname} />) 
            });
            break;

          case 'lineart':
            console.log(reply.message);
            // this.canvas.loadSaveData(reply.message)
            // this.setState({ 
            //   messages: this.messages.push(<Message message={reply.type + ': ' + reply.message} sender="" />) 
            // });
            this.canvas.loadSaveData(reply.message);
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

  UndoLine = async (e) => {
    e.preventDefault();
    this.canvas.undo();
  }

  DrawLine = async (e) => {
    // e.preventDefault();
    this.canvas.loadSaveData();
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
          <button>Отменить</button>
        </form>
        <div>
          <div className="chat" ref={(div) => { this.messageList = div; }}>
            {this.messages}
          </div>
          {this.role == 'Угадывающий' && <Form sendMessage={this.sendMessage}/>}
          <Suggestions sendSuggestion={this.sendSuggestion}/>
        </div>
      </div>
    );
  }
}

export default App