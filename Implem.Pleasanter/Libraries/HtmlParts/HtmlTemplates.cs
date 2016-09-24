﻿using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Images;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using System;
using System.Web;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlTemplates
    {
        public static HtmlBuilder Template(
            this HtmlBuilder hb,
            Permissions.Types permissionType,
            Versions.VerTypes verType,
            BaseModel.MethodTypes methodType,
            bool allowAccess,
            long siteId = 0,
            long parentId = 0,
            string referenceType = "",
            string title = "",
            bool useBreadcrumb = true,
            bool useTitle = true,
            bool useSearch = true,
            bool useNavigationMenu = true,
            string script = "",
            string userScript = "",
            string userStyle = "",
            Action action = null)
        {
            return hb
                .MainContainer(
                    permissionType: permissionType,
                    verType: verType,
                    methodType: methodType,
                    allowAccess: allowAccess,
                    siteId: siteId,
                    parentId: parentId,
                    referenceType: referenceType,
                    title: title,
                    useBreadcrumb: useBreadcrumb,
                    useTitle: useTitle,
                    useSearch: useSearch,
                    useNavigationMenu: useNavigationMenu,
                    userStyle: userStyle,
                    action: action)
                .Scripts(
                    script: script,
                    userScript: userScript,
                    referenceType: referenceType)
                .Hidden(controlId: "Language", value: Sessions.Language());
        }

        public static HtmlBuilder MainContainer(
            this HtmlBuilder hb,
            Permissions.Types permissionType,
            Versions.VerTypes verType,
            BaseModel.MethodTypes methodType,
            bool allowAccess,
            long siteId,
            long parentId,
            string referenceType,
            string title,
            bool useBreadcrumb = true,
            bool useTitle = true,
            bool useSearch = true,
            bool useNavigationMenu = true,
            string userStyle = "",
            Action action = null)
        {
            return hb.Div(id: "MainContainer", action: () => hb
                .Header(
                    permissionType: permissionType,
                    siteId: siteId,
                    referenceType: referenceType,
                    useSearch: useSearch,
                    allowAccess: allowAccess,
                    useNavigationMenu: useNavigationMenu)
                .Article(id: "Application", action: () =>
                {
                    if (allowAccess)
                    {
                        hb.Nav(css: "both cf", action: () => hb
                            .Breadcrumb(
                                siteId: siteId,
                                permissionType: permissionType,
                                _using: useBreadcrumb));
                        if (useTitle)
                        {
                            hb.Title(permissionType: permissionType, siteId: siteId, text: title);
                        }
                        action();
                        hb.P(id: "Message", css: "message", action: () => hb
                            .Raw(text: SessionMessage()));
                    }
                    else
                    {
                        hb
                            .P(id: "Message", css: "message", action: () => hb
                                .Raw(text: Messages.NotFound().Html))
                            .MainCommands(
                                siteId: siteId,
                                permissionType: permissionType,
                                verType: Versions.VerTypes.Latest);
                    }
                })
                .Div(id: "BottomMargin both")
                .Footer(id: "Footer", action: () => hb
                    .P(action: () => hb
                        .A(
                            attributes: new HtmlAttributes().Href(Parameters.General.HtmlCopyrightUrl),
                            action: () => hb
                                .Raw(Parameters.General.HtmlCopyright.Params(DateTime.Now.Year)))))
                .Hidden(controlId: "ApplicationPath", value: Navigations.Get())
                .BackUrl(siteId: siteId, parentId: parentId)
                .Styles(style: userStyle));
        }

        private static HtmlBuilder Title(
            this HtmlBuilder hb,
            Permissions.Types permissionType,
            long siteId,
            string text)
        {
            if (text != string.Empty)
            {
                var binaryModel = new BinaryModel(permissionType, siteId);
                if (binaryModel.ExistsSiteImage(ImageData.SizeTypes.Icon))
                {
                    hb.Img(
                        src: Navigations.Get(
                            "Items",
                            siteId.ToString(),
                            "Binaries",
                            "SiteImageIcon",
                            binaryModel.SiteImagePrefix(ImageData.SizeTypes.Icon)),
                        css: "site-image-icon");
                }
                return hb.Header(id: "HeaderTitleContainer", action: () => hb
                    .H(number: 1, id: "HeaderTitle", action: () => hb
                        .Text(text: text)));
            }
            else
            {
                return hb;
            }
        }

        private static string SessionMessage()
        {
            var html = Sessions.Data("Message");
            if (html != string.Empty)
            {
                Sessions.Clear("Message");
                return html;
            }
            else
            {
                return string.Empty;
            }
        }

        public static HtmlBuilder NotFoundTemplate(this HtmlBuilder hb)
        {
            return hb.Template(
                permissionType: Permissions.Types.NotSet,
                verType: Versions.VerTypes.Latest,
                methodType: BaseModel.MethodTypes.NotSet,
                allowAccess: false,
                useBreadcrumb: false,
                useTitle: false,
                useNavigationMenu: false);
        }

        private static HtmlBuilder BackUrl(this HtmlBuilder hb, long siteId, long parentId)
        {
            return !Request.IsAjax()
                ? hb.Hidden(controlId: "BackUrl", rawValue: BackUrl(siteId, parentId))
                : hb;
        }

        private static string BackUrl(long siteId, long parentId)
        {
            var controller = Routes.Controller();
            var referer = HttpUtility.UrlDecode(new Request(HttpContext.Current).UrlReferrer());
            switch (controller)
            {
                case "admins":
                    return Navigations.Top();
                case "depts":
                case "users":
                    return Routes.Action() == "edit"
                        ? Strings.CoalesceEmpty(referer, Navigations.Get(controller))
                        : Navigations.Get("Admins");
                default:
                    switch (Routes.Action())
                    {
                        case "new":
                        case "edit":
                            return Strings.CoalesceEmpty(referer, Navigations.ItemIndex(siteId));
                        default:
                            return Navigations.ItemIndex(parentId);
                    }
            }
        }
    }
}