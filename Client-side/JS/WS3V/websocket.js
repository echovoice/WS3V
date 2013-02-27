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
	  protocol_version: 1,
    _socket: {},
    latency: -1,
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
	
	session_id: '',
	
	heart:
	{
		beat:-1,
		busy:false,
		lub:null,
		pacemaker:null
	},

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
		
			data = JSON.stringify(data);
	
		this._socket.send(data);
	
		// we need to check and remove the heartbeat timeouts
		if(this.heart.beat != -1 && !this.heart.busy)
		{
			clearTimeout(this.heart.pacemaker);
	
			var that = this;
			this.heart.pacemaker = 	setTimeout(function()
			{
				that.heart.lub = (new Date()).getTime();
				that.Send("lub");
			}, that.heart.beat);
		}
	
		if (this.settings.DebugMode)
		
			this.Debug((data != 'lub') ? vkbeautify.json(data) : 'lub <3', 'client');
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

		// check for heartbeat message
		if(event.data == 'dub')
		{
			// this should always be true but just in case
			if(this.heart.beat != -1)
			{
				this.latency = (((new Date()).getTime()) - this.heart.lub);
				
				// if we can't always be checking the tunnel pulse then
				// we need to make sure we are within the interval
				// otherwise the interval pacemaker should already be alive and kicking
				if(!this.heart.busy)
				{
					var that = this;
					this.heart.pacemaker = 	setTimeout(function()
					{
						that.heart.lub = (new Date()).getTime();
						that.Send("lub");
					}, that.heart.beat);
				}
				
				// show debug output
				if (this.settings.DebugMode)
				{
		  			this.Debug('dub <3', 'server');
					this.Debug('[ws3v diagnostics -> connection latency: ' + this.latency + 'ms]', 'client');
				}
			}
			
			// no need to go any further
			return;
		}

      if (this.settings.DebugMode)
		  this.Debug(vkbeautify.json(event.data), 'server');
		
		var data = JSON.parse(event.data);
		
	
		// if we are not authenticated then either we need to respond to a gatekeeper
		// or the message could be the howdy which means we are authorized
		
		if(!this.authenticated)
		{
			// this is a gatekeeper request, respond with signature
			if(data[0] == 1)
			{
				var response = WS3VWebSocket.prototype.signature;
				response.credentials = this.settings.Credentials;
				this.Send(response.ToString());
			}
			
			// this message is a howdy, meaning the server is ok talking to us
			else if(data[0] == 3)
			{
				// validate server protocol versions match
				if(this.protocol_version != data[2])
				{
					this._OnClose();
					throw 'UNSUPPORTED: WS3V Protocol Version Mismatch, Client: ' + this.protocol_version + ', Server: ' + data[2];
					return;
				}
				
				// process howdy message
				// get session id information
				this.session_id = data[1];
				
				// check if heartbeats are enabled
				if(data[4][0] >= 0)
				{
					// calculate the heartbeat interval
					// we are using the average between the low and high
					// this is very conservative calculation and can be tweeked
					this.heart.beat = Math.round(Math.abs(data[4][1] - data[4][0]) / 2) * 1000;
					this.heart.busy = data[4][2];
					
					// seems the server doesnt care if we heartbeat at a steady interval
					// to collect latency diagnostic data.. so lets do it!
					if(this.heart.busy)
					{
						var that = this;
						this.heart.pacemaker = 	setInterval(function()
						{
							that.heart.lub = (new Date()).getTime();
							that.Send("lub");
						}, that.heart.beat);
					}
					
					// we need to setup the timeout mode
					else
					{
						var that = this;
						this.heart.pacemaker = 	setTimeout(function()
						{
							that.heart.lub = (new Date()).getTime();
							that.Send("lub");
						}, that.heart.beat);
					}
				}
				
				this.authenticated = true;
				
			}
			
			// else.... this isnt good, the server might not speak WS3V
			// so its safe to abort the connection
			
			else
				this._OnClose();
			
			// if this is an aux message, meaning the client code needs to communicate directly with the
			// server, then we dont fire the call back since it never really happened
			// we should always return at this point
			return;
		}
		
		
		
		
      	this.MessageReceived(data);
    },

	_OnClose: function()
	{
		var instance = this;
		
		if (this.settings.DebugMode)
			this.Debug('Connection closed.', 'client');

		this.SocketState = WS3VWebSocket.prototype.SocketStates.Closed;

		this.Disconnected();
	  
		// we need to check and remove the heartbeat intervals
		if(this.heart.beat != -1)
		{
			// clear the timeout or interval
			if(this.heart.busy)
				clearInterval(this.heart.pacemaker);
				
			else
				clearTimeout(this.heart.pacemaker);
		}
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