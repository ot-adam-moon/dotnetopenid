﻿//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Andrew Arnott">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DotNetOpenAuth.OpenIdOfflineProvider {
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Diagnostics.Contracts;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Text;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Data;
	using System.Windows.Documents;
	using System.Windows.Input;
	using System.Windows.Media;
	using System.Windows.Media.Imaging;
	using System.Windows.Navigation;
	using System.Windows.Shapes;
	using log4net;
	using log4net.Appender;
	using log4net.Core;

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, IDisposable {
		/// <summary>
		/// The OpenID Provider host object.
		/// </summary>
		private HostedProvider hostedProvider = new HostedProvider();

		/// <summary>
		/// The logger the application may use.
		/// </summary>
		private ILog logger = log4net.LogManager.GetLogger(typeof(MainWindow));

		/// <summary>
		/// Initializes a new instance of the <see cref="MainWindow"/> class.
		/// </summary>
		public MainWindow() {
			this.InitializeComponent();
			this.hostedProvider.ProcessAuthenticationRequest = request => CheckIdWindow.ProcessAuthentication(this.hostedProvider, request);
			TextWriterAppender boxLogger = log4net.LogManager.GetRepository().GetAppenders().OfType<TextWriterAppender>().FirstOrDefault(a => a.Name == "TextBoxAppender");
			if (boxLogger != null) {
				boxLogger.Writer = new TextBoxTextWriter(logBox);
			}

			this.startProvider();
		}

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose() {
			this.Dispose(true);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				var host = this.hostedProvider as IDisposable;
				if (host != null) {
					host.Dispose();
				}

				this.hostedProvider = null;
			}
		}

		#endregion

		/// <summary>
		/// Raises the <see cref="E:Closing"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
			this.stopProvider();
			base.OnClosing(e);
		}

		/// <summary>
		/// Starts the provider.
		/// </summary>
		private void startProvider() {
			this.hostedProvider.StartProvider();
			this.opIdentifierLabel.Content = this.hostedProvider.OPIdentifier;
		}

		/// <summary>
		/// Stops the provider.
		/// </summary>
		private void stopProvider() {
			this.hostedProvider.StopProvider();
			this.opIdentifierLabel.Content = string.Empty;
		}

		/// <summary>
		/// Handles the MouseDown event of the opIdentifierLabel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
		private void opIdentifierLabel_MouseDown(object sender, MouseButtonEventArgs e) {
			Clipboard.SetText(opIdentifierLabel.Content.ToString());
		}
	}
}
