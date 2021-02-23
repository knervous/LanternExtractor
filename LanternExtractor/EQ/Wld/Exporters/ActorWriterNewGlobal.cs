﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using LanternExtractor.EQ.Wld.Fragments;
using LanternExtractor.EQ.Wld.Helpers;

namespace LanternExtractor.EQ.Wld.Exporters
{
    public class ActorWriterNewGlobal : TextAssetWriter
    {
        private ActorType _actorType;
        private int _actorCount;
        
        private List<string> _objects = new List<string>();

        public ActorWriterNewGlobal(ActorType actorType)
        {
            _actorType = actorType;
            
            string filePath = $"equipment/actors_equipment_{_actorType.ToString().ToLower()}.txt";
            if (!File.Exists(filePath))
            {
                return;
            }
            
            var text = File.ReadAllLines(filePath);

            foreach (var line in text)
            {
                _objects.Add(line);
            }
        }
        
        public override void AddFragmentData(WldFragment data)
        {
            Actor actor = data as Actor;

            if (actor == null)
            {
                return;
            }
            
            if (actor.ActorType != _actorType)
            {
                return;
            }
            
            StringBuilder newActor = new StringBuilder();

            if (_actorType == ActorType.Skeletal)
            {
                newActor.Append(FragmentNameCleaner.CleanName(actor));
                newActor.Append(",");
                newActor.Append(FragmentNameCleaner.CleanName(actor.SkeletonReference.SkeletonHierarchy));
            }
            else if (_actorType == ActorType.Static)
            {
                newActor.Append(FragmentNameCleaner.CleanName(actor));
                newActor.Append(",");

                if (actor.MeshReference.Mesh != null)
                {
                    newActor.Append(FragmentNameCleaner.CleanName(actor.MeshReference.Mesh));
                }
                else if (actor.MeshReference.AlternateMesh != null)
                {
                    newActor.Append(FragmentNameCleaner.CleanName(actor.MeshReference.AlternateMesh));
                }
            }
            else
            {
                newActor.Append(FragmentNameCleaner.CleanName(actor));
            }
            
            _objects.Add(newActor.ToString());
            
            _actorCount++;
        }

        public override void WriteAssetToFile(string fileName)
        {
            _objects.Sort();
            
            foreach (var o in _objects)
            {
                _export.AppendLine(o);
            }
            
            //StringBuilder headerBuilder = new StringBuilder();
            //headerBuilder.AppendLine(LanternStrings.ExportHeaderTitle +
                                     //"Actor List");
            //headerBuilder.AppendLine("# Total models: " + _actorCount);
            //_export.Insert(0, headerBuilder.ToString());
            
            base.WriteAssetToFile(fileName);
        }
    }
}