using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace TP3
{
    /**/
    public static class Stockage
    {
        private static readonly JsonSerializerOptions Options =
            new(JsonSerializerDefaults.Web) { WriteIndented = true };

        public static async Task SauverAsync(string chemin, List<Note> notes)
        {
           string json = JsonSerializer.Serialize(notes, Options);
           await File.WriteAllTextAsync(chemin, json);

        }
        public static async Task <List<Note>> ChargerAsync(string chemin)
        {
            string json = await File.ReadAllTextAsync(chemin);
            return JsonSerializer.Deserialize<List<Note>>(json, Options) ?? [];
        }
    }
}
