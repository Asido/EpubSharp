﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using EpubSharp.Format;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EpubSharp.Tests
{
    [TestClass]
    public class EpubReaderTests
    {
        [TestMethod]
        public void OpenEpub30Test()
        {
            ZipAndMoveAndTestEpubOpen(@"../../Samples/epub30");
        }

        [TestMethod]
        public void OpenEpub31Test()
        {
            ZipAndMoveAndTestEpubOpen(@"../../Samples/epub31");
        }

        [TestMethod]
        public void OpenEpubAssortedTest()
        {
            MoveAndTestEpubOpen(@"../../Samples/epub-assorted");
        }

        [TestMethod]
        public void EpubAsPlainTextTest1()
        {
            var book = EpubReader.Read(@"../../Samples/epub-assorted/boothbyg3249432494-8epub.epub");
            //File.WriteAllText("../../Samples/epub-assorted/boothbyg3249432494-8epub.txt", book.ToPlainText());

            Func<string, string> normalize = text => text.Replace("\r", "").Replace("\n", "").Replace(" ", "");
            var expected = File.ReadAllText(@"../../Samples/epub-assorted/boothbyg3249432494-8epub.txt");
            var actual = book.ToPlainText();
            Assert.AreEqual(normalize(expected), normalize(actual));

            var lines = actual.Split('\n').Select(str => str.Trim()).ToList();
            Assert.IsNotNull(lines.SingleOrDefault(e => e == "I. KAPITEL."));
            Assert.IsNotNull(lines.SingleOrDefault(e => e == "II. KAPITEL."));
            Assert.IsNotNull(lines.SingleOrDefault(e => e == "III. KAPITEL."));
            Assert.IsNotNull(lines.SingleOrDefault(e => e == "IV. KAPITEL."));
            Assert.IsNotNull(lines.SingleOrDefault(e => e == "V. KAPITEL."));
            Assert.IsNotNull(lines.SingleOrDefault(e => e == "VI. KAPITEL."));
            Assert.IsNotNull(lines.SingleOrDefault(e => e == "VII. KAPITEL."));
            Assert.IsNotNull(lines.SingleOrDefault(e => e == "VIII. KAPITEL."));
            Assert.IsNotNull(lines.SingleOrDefault(e => e == "IX. KAPITEL."));
            Assert.IsNotNull(lines.SingleOrDefault(e => e == "X. KAPITEL."));
            Assert.IsNotNull(lines.SingleOrDefault(e => e == "XI. KAPITEL."));
            Assert.IsNotNull(lines.SingleOrDefault(e => e == "XII. KAPITEL."));
            Assert.IsNotNull(lines.SingleOrDefault(e => e == "XIII. KAPITEL."));
            Assert.IsNotNull(lines.SingleOrDefault(e => e == "XIV. KAPITEL."));
            Assert.IsNotNull(lines.SingleOrDefault(e => e == "XV. KAPITEL."));
            Assert.IsNotNull(lines.SingleOrDefault(e => e == "XVI. KAPITEL."));
            Assert.IsNotNull(lines.SingleOrDefault(e => e == "XVII. KAPITEL."));
        }

        [TestMethod]
        public void EpubAsPlainTextTest2()
        {
            var book = EpubReader.Read(@"../../Samples/epub-assorted/iOS Hackers Handbook.epub");
            //File.WriteAllText("../../Samples/epub-assorted/iOS Hackers Handbook.txt", book.ToPlainText());

            Func<string, string> normalize = text => text.Replace("\r", "").Replace("\n", "").Replace(" ", "");
            var expected = File.ReadAllText(@"../../Samples/epub-assorted/iOS Hackers Handbook.txt");
            var actual = book.ToPlainText();
            Assert.AreEqual(normalize(expected), normalize(actual));
            
            var trimmed = string.Join("\n", actual.Split('\n').Select(str => str.Trim()));
            Assert.AreEqual(1, Regex.Matches(trimmed, "Chapter 1\niOS Security Basics").Count);
            Assert.AreEqual(1, Regex.Matches(trimmed, "Chapter 2\niOS in the Enterprise").Count);
            Assert.AreEqual(1, Regex.Matches(trimmed, "Chapter 3\nEncryption").Count);
            Assert.AreEqual(1, Regex.Matches(trimmed, "Chapter 4\nCode Signing and Memory Protections").Count);
            Assert.AreEqual(1, Regex.Matches(trimmed, "Chapter 5\nSandboxing").Count);
            Assert.AreEqual(1, Regex.Matches(trimmed, "Chapter 6\nFuzzing iOS Applications").Count);
            Assert.AreEqual(1, Regex.Matches(trimmed, "Chapter 7\nExploitation").Count);
            Assert.AreEqual(1, Regex.Matches(trimmed, "Chapter 8\nReturn-Oriented Programming").Count);
            Assert.AreEqual(1, Regex.Matches(trimmed, "Chapter 9\nKernel Debugging and Exploitation").Count);
            Assert.AreEqual(1, Regex.Matches(trimmed, "Chapter 10\nJailbreaking").Count);
            Assert.AreEqual(1, Regex.Matches(trimmed, "Chapter 11\nBaseband Attacks").Count);
            Assert.AreEqual(1, Regex.Matches(trimmed, "How This Book Is Organized").Count);
            Assert.AreEqual(2, Regex.Matches(trimmed, "Appendix: Resources").Count);
            Assert.AreEqual(2, Regex.Matches(trimmed, "Case Study: Pwn2Own 2010").Count);
        }

        private void ZipAndMoveAndTestEpubOpen(string samplePath)
        {
            if (samplePath == null) throw new ArgumentNullException(nameof(samplePath));

            var destination = Path.Combine("Samples", Path.GetFileName(samplePath));
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            var samples = Directory.GetDirectories(samplePath, "*", SearchOption.TopDirectoryOnly).ToList();
            var archives = new List<string>();

            foreach (var source in samples)
            {
                var archiveName = Path.GetFileName(source) + ".zip";
                var archivePath = Path.Combine(destination, archiveName);
                if (!File.Exists(archivePath))
                {
                    ZipFile.CreateFromDirectory(source, archivePath);
                }
                archives.Add(archivePath);
            }

            OpenEpubTest(archives);
        }

        private void MoveAndTestEpubOpen(string samplePath)
        {
            if (samplePath == null) throw new ArgumentNullException(nameof(samplePath));

            var destination = Path.Combine("Samples", Path.GetFileName(samplePath));
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            var samples = Directory.GetFiles(samplePath, "*.epub");
            var archives = new List<string>();

            foreach (var source in samples)
            {
                var archiveName = Path.GetFileName(source);
                var archivePath = Path.Combine(destination, archiveName);
                if (!File.Exists(archivePath))
                {
                    File.Copy(source, archivePath);
                }
                archives.Add(archivePath);
            }

            OpenEpubTest(archives);
        }

        private void OpenEpubTest(ICollection<string> files)
        {
            var exceptions = new List<string>();

            foreach (var path in files)
            {
                try
                {
                    EpubReader.Read(path);
                    //AssertOcf(book.Format.Ocf, book.Format.NewOcf);
                    //AssertOpf(book.Format.Opf, book.Format.NewOpf);
                    //AssertNcx(book.Format.Ncx, book.Format.NewNcx);
                }
                catch (Exception ex)
                {
                    exceptions.Add($"Failed to open book: '{path}'. Exception: {ex.Message}");
                }
            }

            if (exceptions.Any())
            {
                var message = $"Failed to open {exceptions.Count}/{files.Count} samples.{Environment.NewLine}{string.Join(Environment.NewLine, exceptions)}";
                Assert.Fail(message);
            }
        }

        /*
        private void AssertNcx(NcxDocument expected, NcxDocument actual)
        {
            Assert.AreEqual(expected == null, actual == null, nameof(actual));
            if (expected != null && actual != null)
            {
                Assert.AreEqual(expected.DocAuthor, actual.DocAuthor, nameof(actual.DocAuthor));
                Assert.AreEqual(expected.DocTitle, actual.DocTitle, nameof(actual.DocTitle));

                AssertCollection(expected.Metadata, actual.Metadata, nameof(actual.Metadata), (old, @new, i) =>
                {
                    Assert.AreEqual(old[i].Name, @new[i].Name, "Metadata.Name");
                    Assert.AreEqual(old[i].Resources, @new[i].Resources, "Metadata.Resources");
                    Assert.AreEqual(old[i].Scheme, @new[i].Scheme, "Metadata.Scheme");
                });

                Assert.AreEqual(expected.NavigationList == null, actual.NavigationList == null, "NavigationList");
                if (expected.NavigationList != null && actual.NavigationList != null)
                {
                    Assert.AreEqual(expected.NavigationList.Id, actual.NavigationList.Id, "NavigationList.Id");
                    Assert.AreEqual(expected.NavigationList.Class, actual.NavigationList.Class, "NavigationList.Class");
                    Assert.AreEqual(expected.NavigationList.Label, actual.NavigationList.Label, "NavigationList.LabelText");

                    AssertCollection(expected.NavigationList.NavigationTargets, actual.NavigationList.NavigationTargets, nameof(actual.NavigationList.NavigationTargets), (old, @new, i) =>
                    {
                        Assert.AreEqual(old[i].Id, @new[i].Id, "NavigationTarget.Id");
                        Assert.AreEqual(old[i].Class, @new[i].Class, "NavigationTarget.Class");
                        Assert.AreEqual(old[i].Label, @new[i].Label, "NavigationTarget.LabelText");
                        Assert.AreEqual(old[i].PlayOrder, @new[i].PlayOrder, "NavigationTarget.PlayOrder");
                        Assert.AreEqual(old[i].ContentSource, @new[i].ContentSource, "NavigationTarget.ContentSrc");
                    });
                }

                AssertCollection(expected.NavigationMap, actual.NavigationMap, nameof(actual.NavigationMap), (old, @new, i) =>
                {
                    Assert.AreEqual(old[i].Id, @new[i].Id, "NavigationMap.Id");
                    Assert.AreEqual(old[i].PlayOrder, @new[i].PlayOrder, "NavigationMap.PlayOrder");
                    Assert.AreEqual(old[i].LabelText, @new[i].LabelText, "NavigationMap.PlayOrder");
                    Assert.AreEqual(old[i].Class, @new[i].Class, "NavigationMap.Class");
                    Assert.AreEqual(old[i].ContentSrc, @new[i].ContentSrc, "NavigationMap.ContentSorce");
                    AssertNavigationPoints(old[i].NavigationPoints, @new[i].NavigationPoints);
                });

                AssertCollection(expected.PageList, actual.PageList, nameof(actual.PageList), (old, @new, i) =>
                {
                    Assert.AreEqual(old[i].Id, @new[i].Id, "PageList.Id");
                    Assert.AreEqual(old[i].Class, @new[i].Class, "PageList.Class");
                    Assert.AreEqual(old[i].ContentSource, @new[i].ContentSource, "PageList.ContentSrc");
                    Assert.AreEqual(old[i].Label, @new[i].Label, "PageList.LabelText");
                    Assert.AreEqual(old[i].Type, @new[i].Type, "PageList.Type");
                    Assert.AreEqual(old[i].Value, @new[i].Value, "PageList.Value");
                });
            }
        }

        private void AssertNavigationPoints(IEnumerable<NcxNavigationPoint> expected, IEnumerable<NcxNavigationPoint> actual)
        {
            AssertCollection(expected, actual, "NavigationPoint", (old, @new, i) =>
            {
                Assert.AreEqual(old[i].Id, @new[i].Id, "NavigationPoint.Id");
                Assert.AreEqual(old[i].Class, @new[i].Class, "NavigationPoint.Class");
                Assert.AreEqual(old[i].ContentSrc, @new[i].ContentSrc, "NavigationPoint.ContentSrc");
                Assert.AreEqual(old[i].LabelText, @new[i].LabelText, "NavigationPoint.LabelText");
                Assert.AreEqual(old[i].PlayOrder, @new[i].PlayOrder, "NavigationPoint.PlayOrder");
                Assert.AreEqual(old[i].NavigationPoints == null, @new[i].NavigationPoints == null, "NavigationPoint.NavigationPoints");
                if (old[i].NavigationPoints != null && @new[i].NavigationPoints != null)
                {
                    AssertNavigationPoints(old[i].NavigationPoints, @new[i].NavigationPoints);
                }
            });
        }

        private void AssertOcf(OcfDocument expected, OcfDocument actual)
        {
            Assert.AreEqual(expected.RootFile, actual.RootFile);
        }

        private void AssertOpf(OpfDocument expected, OpfDocument actual)
        {
            Assert.AreEqual(expected == null, actual == null, nameof(actual));
            if (expected != null && actual != null)
            {
                Assert.AreEqual(expected.EpubVersion, actual.EpubVersion, nameof(actual.EpubVersion));

                Assert.AreEqual(expected.Metadata == null, actual.Metadata == null, nameof(actual.Metadata));
                if (expected.Metadata != null && actual.Metadata != null)
                {
                    AssertCreators(expected.Metadata.Creators, actual.Metadata.Creators, nameof(actual.Metadata.Creators));
                    AssertCreators(expected.Metadata.Contributors, actual.Metadata.Contributors, nameof(actual.Metadata.Contributors));

                    AssertCollection(expected.Metadata.Dates, actual.Metadata.Dates, nameof(actual.Metadata.Dates), (old, @new, i) =>
                    {
                        Assert.AreEqual(old[i].Text, @new[i].Text, "Date.Text");
                        Assert.AreEqual(old[i].Event, @new[i].Event, "Date.Event");
                    });

                    AssertCollection(expected.Metadata.Identifiers, actual.Metadata.Identifiers, nameof(actual.Metadata.Identifiers), (old, @new, i) =>
                    {
                        Assert.AreEqual(old[i].Id, @new[i].Id, "Identifier.Id");
                        Assert.AreEqual(old[i].Scheme, @new[i].Scheme, "Identifier.Scheme");
                        Assert.AreEqual(old[i].Text, @new[i].Text, "Identifier.Text");
                    });

                    AssertCollection(expected.Metadata.Metas, actual.Metadata.Metas, nameof(actual.Metadata.Metas), (old, @new, i) =>
                    {
                        Assert.AreEqual(old[i].Id, @new[i].Id, "Meta.Id");
                        Assert.AreEqual(old[i].Name, @new[i].Name, "Meta.Name");
                        Assert.AreEqual(old[i].Property, @new[i].Property, "Meta.Property");
                        Assert.AreEqual(old[i].Refines, @new[i].Refines, "Meta.Refines");
                        Assert.AreEqual(old[i].Scheme, @new[i].Scheme, "Meta.Scheme");
                        Assert.AreEqual(old[i].Text, @new[i].Text, "Meta.Text");
                    });

                    AssertPrimitiveCollection(expected.Metadata.Coverages, actual.Metadata.Coverages, "Coverages", "Coverage");
                    AssertPrimitiveCollection(expected.Metadata.Descriptions, actual.Metadata.Descriptions, "Descriptions", "Description");
                    AssertPrimitiveCollection(expected.Metadata.Languages, actual.Metadata.Languages, "Languages", "Language");
                    AssertPrimitiveCollection(expected.Metadata.Publishers, actual.Metadata.Publishers, "Publishers", "Publisher");
                    AssertPrimitiveCollection(expected.Metadata.Relations, actual.Metadata.Relations, "Relations", "Relation");
                    AssertPrimitiveCollection(expected.Metadata.Rights, actual.Metadata.Rights, "Rights", "Right");
                    AssertPrimitiveCollection(expected.Metadata.Sources, actual.Metadata.Sources, "Sources", "Source");
                    AssertPrimitiveCollection(expected.Metadata.Subjects, actual.Metadata.Subjects, "Subjects", "Subject");
                    AssertPrimitiveCollection(expected.Metadata.Titles, actual.Metadata.Titles, "Titles", "Title");
                    AssertPrimitiveCollection(expected.Metadata.Types, actual.Metadata.Types, "Types", "Type");
                }

                Assert.AreEqual(expected.Guide == null, actual.Guide == null, nameof(actual.Guide));
                if (expected.Guide != null && actual.Guide != null)
                {
                    AssertCollection(expected.Guide.References, actual.Guide.References, nameof(actual.Guide.References), (old, @new, i) =>
                    {
                        Assert.AreEqual(old[i].Title, @new[i].Title, "Reference.Title");
                        Assert.AreEqual(old[i].Type, @new[i].Type, "Reference.Type");
                        Assert.AreEqual(old[i].Href, @new[i].Href, "Reference.Href");
                    });
                }

                Assert.AreEqual(expected.Manifest == null, actual.Manifest == null, nameof(actual.Manifest));
                if (expected.Manifest != null && actual.Manifest != null)
                {
                    AssertCollection(expected.Manifest.Items, actual.Manifest.Items, nameof(actual.Manifest.Items), (old, @new, i) =>
                    {
                        Assert.AreEqual(old[i].Fallback, @new[i].Fallback, "Item.Fallback");
                        Assert.AreEqual(old[i].FallbackStyle, @new[i].FallbackStyle, "Item.FallbackStyle");
                        Assert.AreEqual(old[i].Href, @new[i].Href, "Item.Href");
                        Assert.AreEqual(old[i].Id, @new[i].Id, "Item.Id");
                        Assert.AreEqual(old[i].MediaType, @new[i].MediaType, "Item.MediaType");
                        Assert.AreEqual(old[i].RequiredModules, @new[i].RequiredModules, "Item.RequiredModules");
                        Assert.AreEqual(old[i].RequiredNamespace, @new[i].RequiredNamespace, "Item.RequiredNamespace");
                        AssertPrimitiveCollection(old[i].Properties, @new[i].Properties, "Item.Properties", "Item.Property");
                    });
                }

                Assert.AreEqual(expected.Spine == null, actual.Spine == null, nameof(actual.Spine));
                if (expected.Spine != null && actual.Spine != null)
                {
                    Assert.AreEqual(expected.Spine.Toc, actual.Spine.Toc, nameof(actual.Spine.Toc));
                    AssertCollection(expected.Spine.ItemRefs, actual.Spine.ItemRefs, nameof(actual.Spine.ItemRefs), (old, @new, i) =>
                    {
                        Assert.AreEqual(old[i].Id, @new[i].Id, "ItemRef.Id");
                        Assert.AreEqual(old[i].IdRef, @new[i].IdRef, "ItemRef.IdRef");
                        Assert.AreEqual(old[i].Linear, @new[i].Linear, "ItemRef.Linear");
                        AssertPrimitiveCollection(old[i].Properties, @new[i].Properties, "ItemRef.Properties", "ItemRef.Property");
                    });
                }

                Assert.AreEqual(expected.CoverPath, actual.CoverPath, nameof(actual.CoverPath));
                Assert.AreEqual(expected.NavPath, actual.NavPath, nameof(actual.NavPath));
                Assert.AreEqual(expected.NcxPath, actual.NcxPath, nameof(actual.NcxPath));
            }
        }

        private void AssertCreators(IEnumerable<OpfMetadataCreator> expected, IEnumerable<OpfMetadataCreator> actual, string name)
        {
            AssertCollection(expected, actual, name, (old, @new, i) =>
            {
                Assert.AreEqual(old[i].AlternateScript, @new[i].AlternateScript, $"{name}.AlternateScript");
                Assert.AreEqual(old[i].FileAs, @new[i].FileAs, $"{name}.FileAs");
                Assert.AreEqual(old[i].Role, @new[i].Role, $"{name}.Role");
                Assert.AreEqual(old[i].Text, @new[i].Text, $"{name}.Text");
            });
        }

        private void AssertCollection<T>(IEnumerable<T> expected, IEnumerable<T> actual, string name, Action<List<T>, List<T>, int> assert)
        {
            Assert.AreEqual(expected == null, actual == null, name);
            if (expected != null && actual != null)
            {
                var old = expected.ToList();
                var @new = actual.ToList();

                Assert.AreEqual(old.Count, @new.Count, $"{name}.Count");

                for (var i = 0; i < @new.Count; ++i)
                {
                    assert(old, @new, i);
                }
            }
        }

        private void AssertPrimitiveCollection<T>(IEnumerable<T> expected, IEnumerable<T> actual, string collectionName, string unitName)
        {
            AssertCollection(expected, actual, collectionName, (old, @new, i) =>
            {
                Assert.IsTrue(@new.Contains(old[i]), unitName);
            });
        }
        */
    }
}
