/*var start = function ()
{
	var inc = document.getElementById('incomming');
	var wsImpl = window.WebSocket || window.MozWebSocket;
	var form = document.getElementById('sendForm');
	var input = document.getElementById('sendText');
	
	inc.innerHTML += "connecting to server ..<br/>";

	// create a new websocket and connect
	window.ws = new wsImpl('ws://localhost:8181/consoleappsample', 'my-protocol');

	// when data is comming from the server, this metod is called
	ws.onmessage = function (evt) {
		inc.innerHTML += evt.data + '<br/>';
	};

	// when the connection is established, this method is called
	ws.onopen = function () {
		inc.innerHTML += '.. connection open<br/>';
	};

	// when the connection is closed, this method is called
	ws.onclose = function () {
		inc.innerHTML += '.. connection closed<br/>';
	}
	
	form.addEventListener('submit', function(e){
		e.preventDefault();
		var val = input.value;
		ws.send(val);
		input.value = "";
	});
	
}

window.onload = start;*/

(function() {
  function WS3VWebSocket(options)
  {
    if (!this instanceof WS3VWebSocket)
	{
      return new WS3VWebSocket(options);
    }
	else
	{
      if (!options)
	  {
        options = {};
      }

      this.settings = MergeDefaults(this._defaultOptions, options);

      if (!window.WebSocket)
	  {
        throw 'UNSUPPORTED: Websockets are not supported in this browser!';
      }

      this.SocketState = WS3VWebSocket.prototype.SocketStates.Closed;

      this.Connected = this.settings.Connected;
      this.Disconnected = this.settings.Disconnected;
      this.MessageReceived = this.settings.MessageReceived;
    }
  }

  WS3VWebSocket.prototype =
  {
    _socket: {},
    _lastReceive: (new Date()).getTime(),
    settings: {},

    SocketStates:
	{
      Connecting: 0,
      Open: 1,
      Closing: 2,
      Closed: 3
    },

    SocketState: 3,
	
	authenticated: false,

    Start: function()
	{
      var server = 'ws://' + this.settings.Server + ':' + this.settings.Port + '/' + this.settings.Action;
      var that = this;
      this._socket = new WebSocket(server);
      this._socket.onopen = function() { that._OnOpen(); };
      this._socket.onmessage = function(data) { that._OnMessage(data); };
      this._socket.onclose = function() { that._OnClose(); };
      this.SocketState = WS3VWebSocket.prototype.SocketStates.Connecting;

		if (this.settings.DebugMode)
		{
			if(!window.vkbeautify)
			{
		  		throw "DEBUG: Can't debug without asset vkbeautify.js, debug disabled!";
				this.settings.DebugMode = false;
			}
			else
			{
				// console
				var debugconsole = document.createElement('div');
				debugconsole.setAttribute("id", "debugconsole");
				document.getElementsByTagName('body')[0].appendChild(debugconsole);
				
				// client-side
				var debugclient = document.createElement('div');
				debugclient.setAttribute("id", "client");
				document.getElementById('debugconsole').appendChild(debugclient);
				
				// server-side
				var debugserver = document.createElement('div');
				debugserver.setAttribute("id", "server");
				document.getElementById('debugconsole').appendChild(debugserver);
				
				this.Debug("connecting to " + server, 'client');
			}
		}
    },

    Send: function(data)
	{
      if (typeof data === 'object')
	  {
        data = JSON.stringify(data);
      }

      this._socket.send(data);

      if (this.settings.DebugMode)
	  {
        this.Debug(vkbeautify.json(data), 'client');
      }
    },

	Stop: function()
	{
		this._socket.close();
	
		if (this.settings.DebugMode)
		  this.Debug('Closed connection.', 'client');
	},

    Connected: function() { },
    Disconnected: function() { },
    MessageReceived: function() { },

    _OnOpen: function()
	{
      var instance = this;
      this.SocketState = WS3VWebSocket.prototype.SocketStates.Open;

      if (this.settings.DebugMode)
		  this.Debug('Connected.', 'client');

      this.Connected();
    },

    _OnMessage: function(event)
	{
      var instance = this;

      this._lastReceive = (new Date()).getTime();

      if (this.settings.DebugMode)
		  this.Debug(vkbeautify.json(event.data), 'server');
		
		var data = JSON.parse(event.data);
		
		var response = null;
		
		
		console.log(data);
		console.log(data[0]);
		
		// this is a gatekeeper request, respond with signature
		if(!this.authenticated && data[0] == 1)
		{
			response = WS3VWebSocket.prototype.signature;
			response.credentials = this.settings.Credentials;
		}
		
		if(response != null)
			this.Send(response.ToString());
		else
      		this.MessageReceived(data);
    },

    _OnClose: function()
	{
      var instance = this;
      if (this.settings.DebugMode)
	  	this.Debug('Connection closed.', 'client');

      this.SocketState = WS3VWebSocket.prototype.SocketStates.Closed;

      this.Disconnected();
    },
	
	Debug: function(message, location)
	{ 
		if (this.settings.DebugMode)
			document.getElementById(location).innerHTML = "<pre><code>" + message + "</code></pre>" + document.getElementById(location).innerHTML;
	}
  };

  WS3VWebSocket.prototype._defaultOptions =
  {
    Port: 80,
    Server: '',
    Action: '',

	Credentials: [],

    Connected: function() { },
    Disconnected: function() { },
    MessageReceived: function(data) { },

    DebugMode: false
  };
  
  WS3VWebSocket.prototype.signature =
  {
    id: 2,
    credentials: [],
	
    ToString: function()
	{
		return [this.id, this.credentials];	
	}
  };

  function MergeDefaults(o1, o2)
  {
    var o3 = {};
    var p = {};

    for (p in o1)
      o3[p] = o1[p];

    for (p in o2)
      o3[p] = o2[p];

    return o3;
  }

  window.WS3VWebSocket = WS3VWebSocket;
  window.MergeDefaults = MergeDefaults;

  if(window.MozWebSocket)
    window.WebSocket = MozWebSocket;

})(window);