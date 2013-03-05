﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.People;
using MrCMS.Website;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.Tests.Website
{
    public class CurrentRequestDataTests : MrCMSTest
    {
        public CurrentRequestDataTests()
        {
            A.CallTo(() => CurrentRequestData.CurrentContext.Session)
             .Returns(new FakeHttpSessionState());
        }
        [Fact]
        public void CurrentRequestData_UserGuid_ReturnsTheUserGuidIfTheUserIsLoggedIn()
        {
            var currentUser = new User();
            CurrentRequestData.CurrentUser = currentUser;

            var userGuid = CurrentRequestData.UserGuid;

            userGuid.Should().Be(currentUser.Guid);
        }

        [Fact]
        public void CurrentRequestData_UserGuid_ReturnsTheSessionGuidIfTheUserIsNotLoggedIn()
        {
            CurrentRequestData.CurrentUser = null;
            var newGuid = Guid.NewGuid();
            CurrentRequestData.CurrentContext.Session["current.usersessionGuid"] = newGuid;

            var userGuid = CurrentRequestData.UserGuid;

            userGuid.Should().Be(newGuid);
        }

        [Fact]
        public void CurrentRequestData_UserGuid_IfNotSetBeforeHandShouldGenerateANewGuid()
        {
            CurrentRequestData.CurrentUser = null;

            var userGuid = CurrentRequestData.UserGuid;

            userGuid.Should().NotBeEmpty();
        }
    }

    public class FakeHttpSessionState : HttpSessionStateBase
    {
        public FakeHttpSessionState()
        {
            _store = new SortedList();
        }

        public override void Add(string name, object value)
        {
            _store[name] = value;
        }

        public override object this[int index]
        {
            get { return _store[_store.GetKey(index)]; }
            set
            {
                _store[_store.GetKey(index)] = value;
            }
        }

        public override object this[string name]
        {
            get
            {
                return _store[name];
            }
            set
            {
                _store[name] = value;
            }
        }

        private HttpStaticObjectsCollectionBase _staticObjects;
        private SortedList _store;

        public override HttpStaticObjectsCollectionBase StaticObjects
        {
            get
            {
                return _staticObjects ??
                       (_staticObjects = new HttpStaticObjectsCollectionWrapper(new HttpStaticObjectsCollection()));
            }
        }
    }
}