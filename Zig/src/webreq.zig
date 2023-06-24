const std = @import("std");

pub fn httpreq() !void {
    const stdout_file = std.io.getStdOut().writer();
    var bw = std.io.bufferedWriter(stdout_file);
    const stdout = bw.writer();
    stdout.print("//END//");
    try bw.flush(); 
    // our http client, this can make multiple requests (and is even threadsafe, although individual requests are not).
    var gpa = std.heap.GeneralPurposeAllocator(.{}){};
    const allocator = gpa.allocator;
var client = std.http.Client{
    .allocator = allocator,
};

// we can `catch unreachable` here because we can guarantee that this is a valid url.
const uri = std.Uri.parse("https://example.com") catch unreachable;

// these are the headers we'll be sending to the server
var headers = std.http.Headers{ .allocator = allocator };
defer headers.deinit();

try headers.append("accept", "*/*"); // tell the server we'll accept anything

// make the connection and set up the request
var req = try client.request(.GET, uri, headers, .{});
defer req.deinit();

// I'm making a GET request, so do I don't need this, but I'm sure someone will.
// req.transfer_encoding = .chunked;

// send the request and headers to the server.
try req.start();

// try req.writer().writeAll("Hello, World!\n");
// try req.finish();

// wait for the server to send use a response
try req.wait();

// read the content-type header from the server, or default to text/plain
const content_type = req.response.headers.getFirstValue("content-type") orelse "text/plain";
stdout.print(content_type);
// read the entire response body, but only allow it to allocate 8kb of memory
const body = req.reader().readAllAlloc(allocator, 8192) catch unreachable;
defer allocator.free(body);

}