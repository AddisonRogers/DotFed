const std = @import("std");

pub fn get(url: u8, json: undefined) !u8 {
    const allocator = (std.heap.GeneralPurposeAllocator(.{}){}).allocator; //TODO change allocator
    var client = std.http.Client{
        .allocator = allocator,
    };

    const uri = std.Uri.parse(url) catch unreachable;

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

    // TODO change when GJSON is ready
    const jsonval = try std.json.parse(json, &body, .{});
    return jsonval;
}

pub fn post(url: u8, json: undefined) !void {
    const allocator = (std.heap.GeneralPurposeAllocator(.{}){}).allocator; //TODO change allocator
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
