﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GoogleMapsComponents;
using GoogleMapsComponents.Maps;
using GoogleMapsComponents.Maps.Extension;
using Microsoft.AspNetCore.Components;

namespace ServerSideDemo.Pages
{
    public partial class MapCircleListPage : ComponentBase
    {
        private GoogleMap map1;
        private MapOptions mapOptions;
        int bunchsize = 10;

        private CircleList circleList = null;
        private Dictionary<string, CircleOptions> circleOptionsByRef = new Dictionary<string, CircleOptions>();
        private int lastId = 0;

        protected override void OnInitialized()
        {
            mapOptions = new MapOptions()
            {
                Zoom = 13,
                Center = new LatLngLiteral()
                {
                    Lat = 48.994249,
                    Lng = 12.190451
                },
                MapTypeId = MapTypeId.Roadmap
            };
        }

        /// <summary>
        /// Create a bunch of circles, put them into a dictionary with reference ids and display them on the map.
        /// </summary>
        private async void CreateBunchOfCircles()
        {
            int howMany = this.bunchsize;
            var bounds = await this.map1.InteropObject.GetBounds();
            double maxRadius = (bounds.North - bounds.South) * 111111.0 / (10 + Math.Sqrt(howMany));
            var colors = new string[] { "#FFFFFF", "#9132D1", "#FFD800", "#846A00", "#AAC643", "#C96A00", "#B200FF", "#CD6A00", "#00A321", "#7F6420" };
            var rnd = new Random();
            for (int i = 0; i < howMany; i++)
            {
                var color = colors[rnd.Next(0, colors.Length)];
                var circleOptions = new CircleOptions
                {
                    Map = this.map1.InteropObject,
                    Center = new LatLngLiteral { Lat = bounds.South + rnd.NextDouble() * (bounds.North - bounds.South), Lng = bounds.West + rnd.NextDouble() * (bounds.East - bounds.West) },
                    Radius = (rnd.NextDouble() + 0.2) / 1.2 * maxRadius,
                    StrokeColor = color,
                    StrokeOpacity = 0.60f,
                    StrokeWeight = 2,
                    FillColor = color,
                    FillOpacity = 0.35f,
                    Visible = true,
                    ZIndex = 1000000,
                };
                this.circleOptionsByRef[(++this.lastId).ToString()] = circleOptions;
            }
            await this.RefreshCircleList();
        }

        private async Task RefreshCircleList()
        {
            this.circleList = await CircleList.SyncAsync(this.circleList, this.map1.JsRuntime, circleOptionsByRef, async (ev, sKey, entity) =>
            {
                // Circle has been clicked --> delete it.
                this.circleOptionsByRef.Remove(sKey);
                await this.RefreshCircleList();
            });
        }
    }
}