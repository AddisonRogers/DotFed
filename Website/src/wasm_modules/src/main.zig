const std = @import("std");
const Allocator = std.mem.Allocator;

const Data = struct { data: struct {
    nodes: []struct {
        name: []u8,
        open_sightups: bool,
        platform: struct {
            name: []u8,
        },
    },
} }; // Schema

fn getList() []Data.data.nodes {
    var allocator = std.heap.ArenaAllocator.init(std.heap.page_allocator);
    var x = Data{};
    var json = get("http://localhost:5000", x, allocator); //TODO change when

    return json.data.nodes;
}

const lemmyData = struct {};

const kbinData = struct {};

pub fn get(url: u8, json: undefined, allocator: *Allocator) !json {
    var client = std.http.Client{
        .allocator = allocator,
    };

    var uri: std.Uri = null;

    uri = std.Uri.parse(url) catch unreachable;

    var headers = std.http.Headers{ .allocator = allocator };
    defer headers.deinit();

    try headers.append("accept", "*/*");

    var req = try client.request(.GET, uri, headers, .{});
    defer req.deinit();
    try req.start();
    try req.wait();
    const content_type = req.response.headers.getFirstValue("content-type") orelse "text/plain";
    std.debug.print(content_type);
    const body = req.reader().readAllAlloc(allocator, 8192) catch unreachable;
    defer allocator.free(body);

    json = try std.json.parseFromSliceLeaky(json, allocator, body) catch unreachable;
    return json;
}

pub fn post(url: u8, json: undefined, allocator: *Allocator) !void {
    var client = std.http.Client{
        .allocator = allocator,
    };

    const uri = std.Uri.parse(url) catch unreachable;

    var headers = std.http.Headers{ .allocator = allocator };
    defer headers.deinit();

    try headers.append("accept", "*/*");

    var req = try client.request(.POST, uri, headers, .{});
    defer req.deinit();
    req.transfer_encoding = .chunked;

    try req.start();

    try req.writer().writeAll(json); // POST REQ
    try req.finish();

    try req.wait();

    const content_type = req.response.headers.getFirstValue("content-type") orelse "text/plain";
    std.debug.print("{anytype}", .{content_type});

    // read the entire response body, but only allow it to allocate 8kb of memory
    const body = req.reader().readAllAlloc(allocator, 8192) catch unreachable;
    defer allocator.free(body);
}

export fn exported_func() void {
    std.print(getList());
}

// no wasm in official build as of now as I want to be able to get it out before worrying about performance
