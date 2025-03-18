using FHEMlogs.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace FHEMlogs
{
  /// <summary>
  /// für jeden Sensor feststellen, wann zum letzten Mal Daten gesendet wurden. 
  /// Eine Meldung schreiben, wenn das zu lange her ist,
  /// </summary>
  internal class Program
  {
    private static void Main(string[] args)
    {bool ausgabe = false;
      DirectoryInfo verzeichnis;
      string LogDir;
      FileInfo[] dateien;
      Dictionary<string, DateTime> messwerte = new Dictionary<string, DateTime>();
      if (args.Length > 0)
        LogDir = args[0];
      else
        LogDir = Settings.Default.LogDir; // /opt/fhem/log
      ausgabe = (args.Length > 1);
        verzeichnis = new DirectoryInfo(LogDir);
      if (verzeichnis.Exists)
      {
        dateien = verzeichnis.GetFiles("*.log");
        if (dateien.Count() > 0)
          foreach (FileInfo datei in dateien)
            messwerte = EineDatei(datei, messwerte);  //foreach
                                                      // messwerte enthält jetzt die Sensornamen und die Zeit des letzten
                                                      // Messwertes
        else
          Console.WriteLine("keine Dateien im Verzeichnis");
      }
      else
      {
        Console.WriteLine(verzeichnis.FullName);
        throw new Exception("Verzeichnis existiert nicht");
      }
      DateTime jetzt = DateTime.Now;
      TimeSpan maxAlter = ausgabe ? TimeSpan.FromHours(0) : TimeSpan.FromHours(18);
      foreach (KeyValuePair<string, DateTime> item in messwerte)
      {
        if ((jetzt - item.Value) > maxAlter)
        {
          Console.WriteLine(item);
          Console.WriteLine("letzter Messwert {0}", jetzt - item.Value);
        }
      }
      Console.WriteLine("fertig");
      //Console.ReadKey();
    }

    /// <summary>
    /// eine Datei auswerten
    /// </summary>
    /// <param name="datei">die Datei</param>
    /// 
    /// <param name="messwerte"></param>
    private static Dictionary<string, DateTime> EineDatei(FileInfo datei, Dictionary<string, DateTime> messwerte)
    {
      StreamReader streamReader;
      if (datei == null)
        throw new ArgumentNullException(nameof(datei));
      //Console.WriteLine(datei.Name);
      if (datei.Name.EndsWith("fhem.log"))
        return messwerte;
      if (datei.Name.StartsWith("autocreated-"))
        return messwerte;
      if (datei.Length < 1)
        return messwerte;
      //Console.WriteLine(datei.Length);
      streamReader = File.OpenText(datei.FullName);
      using (TextReader textReader = streamReader)
      {
        while (!streamReader.EndOfStream)
        {
          DateTime messzeit;
          string zeile;
          string[] teile, zeit, datum, datumzeit;
          zeile = textReader.ReadLine();
          teile = zeile.Split(' ');
          if (teile[0].Length != 19)
          {
            Console.WriteLine($"{datei.Name}:\tLänge 1 nicht 19: >{zeile}<");
            continue;
          }
          datumzeit = teile[0].Split('_');
          if (datumzeit.Length != 2)
          {
            Console.WriteLine($"{datei.Name}:\tDatum-Zeit Länge nicht 2: {0}", zeile);
            continue;
          }
          datum = datumzeit[0].Split('-');
          if (datum.Length != 3)
          {
            Console.WriteLine($"{datei.Name}:\tDatum Länge nicht 3: {0}", zeile);
            continue;
          }
          zeit = datumzeit[1].Split(':');
          if (zeit.Length != 3)
          {
            Console.WriteLine($"{datei.Name}:\tZeit Länge nicht 3: {0}", zeile);
            continue;
          }
          messzeit = new DateTime(Convert.ToInt16(datum[0]), Convert.ToInt16(datum[1]), Convert.ToInt16(datum[2]), Convert.ToInt16(zeit[0]), Convert.ToInt16(zeit[1]), Convert.ToInt16(zeit[2]));
          if (messwerte.ContainsKey(teile[1]))
          {
            bool ja;
            ja = messwerte.TryGetValue(teile[1], out DateTime alteZeit);
            if (ja)
              if (messzeit > alteZeit)
              {
                _ = messwerte.Remove(teile[1]);
                messwerte.Add(teile[1], messzeit);
              }
          }
          else
            messwerte.Add(teile[1], messzeit);
        } //while
      } //using
      return messwerte;
    }
  }
}