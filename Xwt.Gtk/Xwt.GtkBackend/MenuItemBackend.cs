// 
// MenuItemBackend.cs
//  
// Author:
//       Lluis Sanchez <lluis@xamarin.com>
// 
// Copyright (c) 2011 Xamarin Inc
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using Xwt.Backends;
using Xwt.Drawing;
using System.Collections.Generic;
using Xwt.Engine;

namespace Xwt.GtkBackend
{
	public class MenuItemBackend: IMenuItemBackend
	{
		IMenuItemEventSink eventSink;
		Gtk.MenuItem item;
		Gtk.Label label;
		List<MenuItemEvent> enabledEvents;
		bool changingCheck;
		
		public MenuItemBackend ()
			: this (new Gtk.ImageMenuItem (""))
		{
		}
		
		public MenuItemBackend (Gtk.MenuItem item)
		{
			this.item = item;
			label = (Gtk.Label) item.Child;
			item.ShowAll ();
		}
		
		public Gtk.MenuItem MenuItem {
			get { return item; }
		}

		public void Initialize (IMenuItemEventSink eventSink)
		{
			this.eventSink = eventSink;
		}

		public void SetSubmenu (IMenuBackend menu)
		{
			if (menu == null)
				item.Submenu = null;
			else {
				Gtk.Menu m = ((MenuBackend)menu).Menu;
				item.Submenu = m;
			}
		}
		
		public void SetImage (object imageBackend)
		{
			Gtk.ImageMenuItem it = item as Gtk.ImageMenuItem;
			if (it == null)
				return;
			if (imageBackend != null) {
				var img = new Gtk.Image ((Gdk.Pixbuf) imageBackend);
				img.ShowAll ();
				it.Image = img;
			}
			else
				it.Image = null;
		}

		public string Label {
			get {
				return label.Text;
			}
			set {
				label.Text = value;
			}
		}
		
		public bool Sensitive {
			get {
				return item.Sensitive;
			}
			set {
				item.Sensitive = value;
			}
		}
		
		public bool Visible {
			get {
				return item.Visible;
			}
			set {
				item.Visible = value;
			}
		}
		
		public bool Checked {
			get { return (item is Gtk.CheckMenuItem) && ((Gtk.CheckMenuItem)item).Active; }
			set {
				if (item is Gtk.CheckMenuItem) {
					changingCheck = true;
					((Gtk.CheckMenuItem)item).Active = value;
					changingCheck = false;
				}
			}
		}
		
/*		public void SetType (MenuItemType type)
		{
			string text = label.Text;
			
			Gtk.MenuItem newItem = null;
			switch (type) {
			case MenuItemType.Normal:
				if (!(item is Gtk.ImageMenuItem))
					newItem = new Gtk.ImageMenuItem (text);
				break;
			case MenuItemType.CheckBox:
				if (item.GetType () != typeof(Gtk.CheckMenuItem))
					newItem = new Gtk.CheckMenuItem (text);
				break;
			case MenuItemType.RadioButton:
				if (!(item is Gtk.RadioMenuItem))
					newItem = new Gtk.RadioMenuItem (text);
				break;
			}
			
			if (newItem != null) {
				if ((newItem is Gtk.CheckMenuItem) && (item is Gtk.CheckMenuItem))
					((Gtk.CheckMenuItem)item).Active = ((Gtk.CheckMenuItem)newItem).Active;
				newItem.Sensitive = item.Sensitive;
				if (item.Parent != null) {
					Gtk.Menu m = (Gtk.Menu)item.Parent;
					int pos = Array.IndexOf (m.Children, item);
					m.Insert (newItem, pos);
					m.Remove (item);
				}
				newItem.ShowAll ();
				if (!item.Visible)
					newItem.Hide ();
				
				if (enabledEvents != null) {
					foreach (var ob in enabledEvents)
						DisableEvent (ob);
				}
				
				item = newItem;
				label = (Gtk.Label) item.Child;
				
				if (enabledEvents != null) {
					foreach (var ob in enabledEvents)
						EnableEvent (ob);
				}
			}
		}*/
		
		public void Initialize (object frontend)
		{
		}

		public void EnableEvent (object eventId)
		{
			if (eventId is MenuItemEvent) {
				if (enabledEvents == null)
					enabledEvents = new List<MenuItemEvent> ();
				enabledEvents.Add ((MenuItemEvent)eventId);
				if ((MenuItemEvent)eventId == MenuItemEvent.Clicked)
					item.Activated += HandleItemActivated;
			}
		}

		public void DisableEvent (object eventId)
		{
			if (eventId is MenuItemEvent) {
				enabledEvents.Remove ((MenuItemEvent)eventId);
				if ((MenuItemEvent)eventId == MenuItemEvent.Clicked)
					item.Activated -= HandleItemActivated;
			}
		}

		void HandleItemActivated (object sender, EventArgs e)
		{
			if (!changingCheck) {
				Toolkit.Invoke (delegate {
					eventSink.OnClicked ();
				});
			}
		}
	}
}

