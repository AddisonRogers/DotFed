const std = @import("std");
const testing = std.testing;
const httpclient = @import("..httpclient.zig").httpclient;

const Data = struct { data: struct {
    nodes: []struct {
        name: []u8,
        open_sightups: bool,
        platform: struct {
            name: []u8,
        },
    },
} };

fn getList() []Data.data.nodes {
    var allocator = std.heap.ArenaAllocator.init(std.heap.page_allocator);
    const query = "query MyQuery {thefederation_node(where: {thefederation_platform: {name: {_iregex: \"lemmy|kbin|mastodon\"}}}) {nameopen_signupsthefederation_platform {name}}}";
    var x = Data{};
    var json = httpclient.get("https://the-federation.info/v1/graphql", x, allocator, query);

    return json.data.nodes;
}
