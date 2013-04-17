(function()
{
	// WS3V WebSocket DEBUG class constructor
	// Built for: ws3v.org
	// https://github.com/echovoice/WS3V/tree/master/Client-side/JS/WS3V/debug-assets
	// Written without jQuery, if you can make a valid reason to convert I will consider it
	
	function WS3VWebSocketDebug()
	{
		// used to determine if heartbeat should show up in the console
		this.showhearts = false;
		
		// array for storing last 10 latencys, used to generate a good average over time
		this.latencys = [];
		
		// which tab is open
		this.tab = 'ws3vdebugreceive';
		
		// console
		var debugconsole = document.createElement('div');
		debugconsole.setAttribute("id", "ws3vdebugconsole");
		debugconsole.setAttribute("data-dragging", 0);
		document.getElementsByTagName('body')[0].appendChild(debugconsole);
		
		// resize bar
		var debugresize = document.createElement('div');
		debugresize.setAttribute("id", "ws3vdebugresizebar");
		document.getElementById('ws3vdebugconsole').appendChild(debugresize);
		
		// top bar
		var debugtop = document.createElement('div');
		debugtop.setAttribute("id", "ws3vdebugtopbar");
		document.getElementById('ws3vdebugconsole').appendChild(debugtop);
		
		// top bar ul
		var debugul = document.createElement('ul');
		debugul.setAttribute("id", "ws3vdebugmenu");
		document.getElementById('ws3vdebugtopbar').appendChild(debugul);
		
		// top bar li close
		var debugclose = document.createElement('li');
		debugclose.setAttribute("id", "ws3vdebugclose");
		debugclose.innerHTML = "<span title=\"Close.\"></span>";
		
		// mac is funny and has the close icon on the left instead of the right, thanks adam
		if(/Mac/.test(navigator.userAgent))
			document.getElementById('ws3vdebugmenu').appendChild(debugclose);
		
		// top bar li receive
		var debugreceive = document.createElement('li');
		debugreceive.setAttribute("id", "ws3vdebugreceive");
		debugreceive.innerHTML = "<i>WebSocket Receive</i>";
		debugreceive.className = "active";
		document.getElementById('ws3vdebugmenu').appendChild(debugreceive);
		
		// top bar li send
		var debugsend = document.createElement('li');
		debugsend.setAttribute("id", "ws3vdebugsend");
		debugsend.innerHTML = "<i>WebSocket Send</i>";
		document.getElementById('ws3vdebugmenu').appendChild(debugsend);
		
		// mac is funny and has the close icon on the left instead of the right, thanks adam
		if(!/Mac/.test(navigator.userAgent))
		{
			debugclose.className = 'pc';
			document.getElementById('ws3vdebugmenu').appendChild(debugclose);
		}
		
		// middle bar
		var debugmiddle = document.createElement('div');
		debugmiddle.setAttribute("id", "ws3vdebugmiddlebar");
		document.getElementById('ws3vdebugconsole').appendChild(debugmiddle);
		
		// client-side
		var debugclient = document.createElement('div');
		debugclient.setAttribute("id", "ws3vdebugclient");
		document.getElementById('ws3vdebugmiddlebar').appendChild(debugclient);
		
		// server-side
		var debugserver = document.createElement('div');
		debugserver.setAttribute("id", "ws3vdebugserver");
		document.getElementById('ws3vdebugmiddlebar').appendChild(debugserver);
		
		// status
		var debugstatus = document.createElement('div');
		debugstatus.setAttribute("id", "ws3vdebugstatus");
		document.getElementsByTagName('body')[0].appendChild(debugstatus);
		
		// status ul
		var debugstatusul = document.createElement('ul');
		debugstatusul.setAttribute("id", "ws3vdebugstatusu");
		document.getElementById('ws3vdebugstatus').appendChild(debugstatusul);
		
		// status li connection
		var debugconnection = document.createElement('li');
		debugconnection.setAttribute("id", "ws3vdebugconnection");
		debugconnection.innerHTML = "<span title=\"Not connected.\"></span>";
		document.getElementById('ws3vdebugstatusu').appendChild(debugconnection);
		
		// status li heart
		var debugheart = document.createElement('li');
		debugheart.setAttribute("id", "ws3vdebugheart");
		debugheart.innerHTML = "<span title=\"Show heartbeats in log.\"></span>";
		document.getElementById('ws3vdebugstatusu').appendChild(debugheart);
		
		// status li clear
		var debugclear = document.createElement('li');
		debugclear.setAttribute("id", "ws3vdebugclear");
		debugclear.innerHTML = "<span title=\"Clear websocket receive log.\"></span>";
		document.getElementById('ws3vdebugstatusu').appendChild(debugclear);
		
		// status li info
		var debuginfo = document.createElement('li');
		debuginfo.setAttribute("id", "ws3vdebuginfo");
		debuginfo.innerHTML = "<span>Average latency: 2ms</span>";
		document.getElementById('ws3vdebugstatusu').appendChild(debuginfo);
		
		
		
		// generate list of children to attach handlers too
		var children = document.getElementById('ws3vdebugconsole').children
		var that = this;

		// itterate over children, this attached the very slight but to me notciable bottom and top border change when
		// the console window is focused on, calling it "highlight"
		for(var i=0; i < children.length; i++)
		{
			children[i].onclick = function(e)
			{
				document.getElementById('ws3vdebugtopbar').style.borderTop = '1px solid #646464';
				document.getElementById('ws3vdebugtopbar').style.borderBottom = '1px solid #646464';
				e.stopPropagation();
			}
		}
		
		// this resets the console "highlight"
		document.getElementsByTagName("body")[0].addEventListener("click", function(e)
		{
			document.getElementById('ws3vdebugtopbar').style.borderTop = '1px solid #A3A3A3';
			document.getElementById('ws3vdebugtopbar').style.borderBottom = '1px solid #A3A3A3';
		}, false);
		
		// changes to the send console
		document.getElementById("ws3vdebugsend").addEventListener("click", function(e)
		{
			that.tab = 'ws3vdebugsend';
			document.getElementById('ws3vdebugclear').innerHTML = "<span title=\"Clear websocket send log.\"></span>";
			document.getElementById('ws3vdebugserver').style.display = 'none';
			document.getElementById('ws3vdebugclient').style.display = 'block';
			document.getElementById('ws3vdebugsend').className = 'active';
			document.getElementById('ws3vdebugreceive').className = '';
		}, false);
		
		// changes to the receive console
		document.getElementById("ws3vdebugreceive").addEventListener("click", function(e)
		{
			that.tab = 'ws3vdebugreceive';
			document.getElementById('ws3vdebugclear').innerHTML = "<span title=\"Clear websocket receive log.\"></span>";
			document.getElementById('ws3vdebugclient').style.display = 'none';
			document.getElementById('ws3vdebugserver').style.display = 'block';
			document.getElementById('ws3vdebugreceive').className = 'active';
			document.getElementById('ws3vdebugsend').className = '';
		}, false);
		
		// clear active console
		document.getElementById("ws3vdebugclear").addEventListener("click", function(e)
		{
			// determine which console is active
			if(that.tab == 'ws3vdebugreceive')
				document.getElementById('ws3vdebugserver').innerHTML = '';
			else
				document.getElementById('ws3vdebugclient').innerHTML = '';
		}, false);
		
		// enables or disables heartbeats
		document.getElementById("ws3vdebugheart").addEventListener("click", function(e)
		{
			if (that.showhearts)
			{
				document.getElementById("ws3vdebugheart").className = "";
				that.showhearts = false;
			}
			else
			{
				document.getElementById("ws3vdebugheart").className = "hearts";
				that.showhearts = true;
			}
				
		}, false);
		
		// detect window blur to reset console "highlight"
		window.addEventListener("blur", function(e)
		{
			document.getElementById('ws3vdebugtopbar').style.borderTop = '1px solid #A3A3A3';
			document.getElementById('ws3vdebugtopbar').style.borderBottom = '1px solid #A3A3A3';
		}, false);
		
		// when the mouse moves across the body, fire this
		document.getElementsByTagName("body")[0].addEventListener("mousemove", function(e)
		{				
			// if we are in drag mode then we need to adjust the height of the console window
			// this will give it the effect of a resize in the y direction
			if(document.getElementById('ws3vdebugconsole').getAttribute("data-dragging") == 1)
			{
				cevent=(typeof event=='undefined'?e:event) 
				var newHeight=parseInt(parseInt(document.getElementById('ws3vdebugconsole').getAttribute("data-curheight"))+parseInt(parseInt(document.getElementById('ws3vdebugconsole').getAttribute("data-curpos"))-cevent.clientY));
				newHeight=(newHeight<80?80:newHeight) 
				document.getElementById('ws3vdebugconsole').style.height=newHeight+'px';
				document.getElementById('ws3vdebugmiddlebar').style.height=(newHeight-53)+'px';
				window.getSelection().removeAllRanges();
			}
			
		}, false);
		
		// close button hook
		document.getElementById("ws3vdebugclose").addEventListener("click", function(e)
		{
			document.getElementById("ws3vdebugconsole").innerHTML = "";
			document.getElementsByTagName('body')[0].removeChild(document.getElementById("ws3vdebugconsole"));
			document.getElementById("ws3vdebugstatus").innerHTML = "";
			document.getElementsByTagName('body')[0].removeChild(document.getElementById("ws3vdebugstatus"));
		});
		
		// we need to determine when the mouse is released to stop the y resize
		document.getElementsByTagName("body")[0].addEventListener("mouseup", function(e)
		{
			// only fire in drag mode
			if(document.getElementById('ws3vdebugconsole').getAttribute("data-dragging") == 1)
			{
				// set drag flag
				document.getElementById('ws3vdebugconsole').setAttribute("data-dragging", 0);
				document.getElementsByTagName("body")[0].style["cursor"] = "auto";
				
				// an effect noticed when a resize is fired but console isnt actually "highlighted"
				if(document.getElementById('ws3vdebugtopbar').style.borderTop != '1px solid rgb(100, 100, 100)')
				{
					setTimeout(function()
					{
						document.getElementById('ws3vdebugtopbar').style.borderTop = '1px solid #A3A3A3';
						document.getElementById('ws3vdebugtopbar').style.borderBottom = '1px solid #A3A3A3';
					}, 1);
				}
			}
			
		}, false);
		
		// detect mouse down on the resize handle
		document.getElementById("ws3vdebugresizebar").addEventListener("mousedown", function(e)
		{
			// set dragging flag
			document.getElementById('ws3vdebugconsole').setAttribute("data-dragging", 1);
			cevent=(typeof event=='undefined'?e:event) 
			document.getElementById('ws3vdebugconsole').setAttribute("data-curpos", cevent.clientY);
			document.getElementById('ws3vdebugconsole').setAttribute("data-curheight", parseInt(document.getElementById('ws3vdebugconsole').offsetHeight));
			// set cursor to resize
			document.getElementsByTagName("body")[0].style["cursor"] = "n-resize";
		}, false);
	}
	
	// Log hook to write to the console
	WS3VWebSocketDebug.prototype.Log = function(location, message)
	{
		if(this.showhearts || (message != "lub" && message != "dub"))
			document.getElementById("ws3vdebug" + location).innerHTML = "<pre><code>" + message + "</code></pre>" + document.getElementById("ws3vdebug" + location).innerHTML;
	};
	
	// Latency hook to add latency values to the rolling average
	WS3VWebSocketDebug.prototype.Latency = function(latency)
	{		
		if(this.latencys.length >= 10)
			this.latencys.shift();
		
		this.latencys.push(latency);
		var l = Math.ceil(this.latencys.reduce(function(a, b) { return a + b; }, 0) /  this.latencys.length);
		document.getElementById('ws3vdebuginfo').innerHTML = "<span>Average latency: " + ((l < 1) ? "< 1" : l) + "ms</span>";
	};
	
	// Closed hook
	WS3VWebSocketDebug.prototype.Closed = function()
	{
		document.getElementById("ws3vdebugconnection").className = "";
		document.getElementById("ws3vdebugconnection").innerHTML = "<span title=\"Not connected.\"></span>"
	};
	
	// Open hook
	WS3VWebSocketDebug.prototype.Open = function(server)
	{
		document.getElementById("ws3vdebuginfo").innerHTML = "<span></span>";
		document.getElementById("ws3vdebugconnection").className = "open";
		document.getElementById("ws3vdebugconnection").innerHTML = "<span title=\"Connected to: " + server + "\"></span>"
	};
	
	// Timeout report, used to display when the connection will try to reconnect
	WS3VWebSocketDebug.prototype.Timeout = function(timeout)
	{
		var left = timeout;
		var attempts = window.setInterval(function()
		{
			left--;
			if(left < 1)
				window.clearInterval(attempts);
			else if(left < 60)
				document.getElementById("ws3vdebuginfo").innerHTML = "<span>Retry in " + left + " second" + ((left != 1) ? "s" : "") + "</span>";
			
			else if(left < 120)
				document.getElementById("ws3vdebuginfo").innerHTML = "<span>Retry in less than 2 minutes</span>";
				
			else
				document.getElementById("ws3vdebuginfo").innerHTML = "<span>Retry in less than 3 minutes</span>";
		}, 1000);
	};
	
	// Connection in progress hook
	WS3VWebSocketDebug.prototype.Connecting = function(server)
	{
		document.getElementById("ws3vdebuginfo").innerHTML = "<span>Connecting to: " + server + "</span>";
	};
	
	// attach the debugger so the websocket class can find it
	window.WS3VWebSocketDebug = new WS3VWebSocketDebug;

})(window);