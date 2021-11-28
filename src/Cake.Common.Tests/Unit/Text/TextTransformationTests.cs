﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using Cake.Common.Tests.Fixtures;
using Cake.Core.IO;
using Cake.Testing;
using NSubstitute;
using Xunit;

namespace Cake.Common.Tests.Unit.Text
{
    public sealed class TextTransformationTests
    {
        public sealed class TheConstructor
        {
            [Fact]
            public void Should_Throw_If_Template_Is_Null()
            {
                // Given
                var fixture = new TextTransformationFixture();
                fixture.TransformationTemplate = null;

                // When
                var result = Record.Exception(() => fixture.CreateTextTransformation());

                // Then
                AssertEx.IsArgumentNullException(result, "template");
            }
        }

        public sealed class TheSaveMethod
        {
            [Fact]
            public void Should_Throw_If_File_Path_Is_Null()
            {
                // Given
                var fixture = new TextTransformationFixture();
                var transformation = fixture.CreateTextTransformation();
                FilePath path = null;

                // When
                var result = Record.Exception(() => transformation.Save(path));

                // Then
                AssertEx.IsArgumentNullException(result, "path");
            }

            [Fact]
            public void Should_Render_Content_To_File_With_Default_Encoding()
            {
                // Given
                var expectedContent = "Hello World";
                var fixture = new TextTransformationFixture();
                fixture.TransformationTemplate.Render().Returns(expectedContent);
                var transformation = fixture.CreateTextTransformation();

                // When
                transformation.Save("./output.txt");

                // Then
                var outputFile = fixture.FileSystem.GetFile("/Working/output.txt");
                Assert.False(outputFile.HasUTF8BOM());
                Assert.Equal(expectedContent, outputFile.GetTextContent());
            }

            [Fact]
            public void Should_Render_Content_To_File_With_Specified_Encoding()
            {
                // Given
                var expectedContent = "Hello World";
                var fixture = new TextTransformationFixture();
                fixture.TransformationTemplate.Render().Returns(expectedContent);
                var transformation = fixture.CreateTextTransformation();

                // When
                transformation.Save("./output.txt", Encoding.UTF8);

                // Then
                var outputFile = fixture.FileSystem.GetFile("/Working/output.txt");
                Assert.True(outputFile.HasUTF8BOM());
                Assert.Equal(expectedContent, outputFile.GetTextContent(Encoding.UTF8));
            }
        }

        public sealed class TheToStringMethod
        {
            [Fact]
            public void Should_Render_The_Provided_Template()
            {
                // Given
                var fixture = new TextTransformationFixture();
                fixture.TransformationTemplate.Render().Returns("Hello World");
                var transformation = fixture.CreateTextTransformation();

                // When
                var result = transformation.ToString();

                // Then
                Assert.Equal("Hello World", result);
            }
        }
    }
}