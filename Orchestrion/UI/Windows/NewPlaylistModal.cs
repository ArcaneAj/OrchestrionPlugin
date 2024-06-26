﻿using System.Collections.Generic;
using CheapLoc;
using ImGuiNET;
using Orchestrion.Persistence;

namespace Orchestrion.UI.Windows;

public class NewPlaylistModal
{
	private string _newPlaylistName = string.Empty;
	private List<int> _newPlaylistSongs = new();
	private bool _isOpen;

	private NewPlaylistModal() { }
	public static NewPlaylistModal Instance { get; } = new();

	public void Show(List<int> songs)
	{
		_newPlaylistSongs = songs;
		_isOpen = true;
	}
	
	public void Close()
	{
		_newPlaylistName = "";
		_newPlaylistSongs = null;
		_isOpen = false;
		ImGui.CloseCurrentPopup();
	}

	public void Draw()
	{
		// This is so stupid
		var loc = Loc.Localize("CreateNewPlaylist", "Create New Playlist");
		
		if (_isOpen)
			ImGui.OpenPopup(loc);
		
		var a = true;
		if (ImGui.BeginPopupModal($"{loc}", ref a, ImGuiWindowFlags.AlwaysAutoResize))
		{
			ImGui.Text(Loc.Localize("EnterPlaylistNameColon", "Enter a name for your playlist:"));
			if (ImGui.IsWindowAppearing())
				ImGui.SetKeyboardFocusHere();
			var yes = ImGui.InputText("##newplaylistname", ref _newPlaylistName, 64, ImGuiInputTextFlags.EnterReturnsTrue);
			var invalid = string.IsNullOrWhiteSpace(_newPlaylistName)
			              || string.IsNullOrEmpty(_newPlaylistName)
			              || Configuration.Instance.Playlists.ContainsKey(_newPlaylistName);
			ImGui.BeginDisabled(invalid);
			yes |= ImGui.Button(Loc.Localize("Create", "Create"));

			if (yes)
			{
				var songs = new List<int>();
				if (_newPlaylistSongs.Count != 0)
					songs.AddRange(_newPlaylistSongs);
				Configuration.Instance.Playlists.Add(_newPlaylistName!, new Playlist(_newPlaylistName, songs));
				Configuration.Instance.Save();
				Close();
			}
			ImGui.EndDisabled();
			ImGui.SameLine();
			if (ImGui.Button(Loc.Localize("Cancel", "Cancel")))
				Close();
			ImGui.EndPopup();
		}
	}
}