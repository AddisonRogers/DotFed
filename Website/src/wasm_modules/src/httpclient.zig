const std = @import("std");
const Allocator = std.mem.Allocator;
const jsonMetaData = struct { len: u8 };
const _Data = @import("..main.zig").Data;

pub fn get(url: u8, json: undefined, allocator: *Allocator, query: anytype) !json {
    var client = std.http.Client{
        .allocator = allocator,
    };

    var uri: std.Uri = null;
    if (@TypeOf(query) != null) {
        uri = std.Uri.parse(url + query) catch unreachable;
    }

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
